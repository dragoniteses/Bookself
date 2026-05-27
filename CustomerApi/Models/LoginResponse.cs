namespace CustomerApi.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public CustomerInfo? Customer { get; set; }
    }
}
