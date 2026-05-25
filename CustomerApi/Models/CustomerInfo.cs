namespace CustomerApi.Models
{
    public class CustomerInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        // Login fields
        public string Username { get; set; } = string.Empty;
        // Stored as Base64 strings
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
    }
}
