using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CustomerApi.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(int customerId, string username);
        int? ExtractCustomerIdFromToken(string token);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly string _secretKey;
        private readonly int _expirationMinutes;

        public JwtTokenService(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            _expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
        }

        public string GenerateToken(int customerId, string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, customerId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim("CustomerId", customerId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "CustomerApi",
                audience: "CustomerApiUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public int? ExtractCustomerIdFromToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = "CustomerApi",
                    ValidateAudience = true,
                    ValidAudience = "CustomerApiUsers",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var customerId = jwtToken.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;

                return int.TryParse(customerId, out var id) ? id : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
