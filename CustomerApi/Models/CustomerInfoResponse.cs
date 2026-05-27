namespace CustomerApi.Models
{
    public class CustomerInfoResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
