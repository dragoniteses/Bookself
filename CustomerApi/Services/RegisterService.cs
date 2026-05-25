using CustomerApi.Data;
using CustomerApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace CustomerApi.Services
{
    public interface IRegisterService
    {
        Task<bool> RegisterAsync(RegisterRequest request);
    }

    public class RegisterService : IRegisterService
    {
        private readonly CustomerContext _db;

        public RegisterService(CustomerContext db)
        {
            _db = db;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            // check for existing username
            if (_db.Customers.Any(c => c.Username == request.Username))
                return false;

            // generate salt and hash using PBKDF2
            var saltBytes = RandomNumberGenerator.GetBytes(16);
            var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(request.Password),
                saltBytes,
                100_000,
                HashAlgorithmName.SHA256,
                32);

            var customer = new CustomerInfo
            {
                Username = request.Username,
                PasswordSalt = Convert.ToBase64String(saltBytes),
                PasswordHash = Convert.ToBase64String(hashBytes),
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
