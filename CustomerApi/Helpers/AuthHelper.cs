using CustomerApi.Services;

namespace CustomerApi.Helpers
{
    public static class AuthHelper
    {
        public static string? ExtractTokenFromHeader(HttpRequest request)
        {
            var authHeader = request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader))
                return null;

            const string bearerScheme = "Bearer ";
            if (!authHeader.StartsWith(bearerScheme, StringComparison.OrdinalIgnoreCase))
                return null;

            return authHeader.Substring(bearerScheme.Length);
        }

        public static int? GetCustomerIdFromRequest(HttpRequest request, IJwtTokenService jwtTokenService)
        {
            var token = ExtractTokenFromHeader(request);
            if (string.IsNullOrEmpty(token))
                return null;

            return jwtTokenService.ExtractCustomerIdFromToken(token);
        }
    }
}
