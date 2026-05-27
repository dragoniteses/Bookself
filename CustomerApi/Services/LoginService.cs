using CustomerApi.Data;
using CustomerApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace CustomerApi.Services
{
    public interface ILoginService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }

    public class LoginService : ILoginService
    {
        private readonly CustomerContext _db;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginService(CustomerContext db, IJwtTokenService jwtTokenService)
        {
            _db = db;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Find customer by username
            var customer = _db.Customers.FirstOrDefault(c => c.Username == request.Username);
            if (customer == null)
                return new LoginResponse { Success = false, Message = "Invalid username or password." };

            // Verify password
            if (!VerifyPassword(request.Password, customer.PasswordHash, customer.PasswordSalt))
                return new LoginResponse { Success = false, Message = "Invalid username or password." };

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(customer.Id, customer.Username);

            // Remove sensitive data before returning
            customer.PasswordHash = string.Empty;
            customer.PasswordSalt = string.Empty;

            return new LoginResponse
            {
                Success = true,
                Message = "Login successful.",
                Customer = customer,
                Token = new TokenResponse
                {
                    Token = token,
                    ExpiresIn = 3600
                }
            };
        }

        private bool VerifyPassword(string password, string passwordHash, string passwordSalt)
        {
            var saltBytes = Convert.FromBase64String(passwordSalt);
            var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                saltBytes,
                100_000,
                HashAlgorithmName.SHA256,
                32);

            var hash = Convert.ToBase64String(hashBytes);
            return hash == passwordHash;
        }
    }
}
