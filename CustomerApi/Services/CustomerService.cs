using CustomerApi.Data;
using CustomerApi.Models;

namespace CustomerApi.Services
{
    public interface ICustomerService
    {
        Task<CustomerInfoResponse?> GetCustomerInfoAsync(int customerId);
    }

    public class CustomerService : ICustomerService
    {
        private readonly CustomerContext _db;

        public CustomerService(CustomerContext db)
        {
            _db = db;
        }

        public async Task<CustomerInfoResponse?> GetCustomerInfoAsync(int customerId)
        {
            var customer = _db.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
                return null;

            return new CustomerInfoResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Username = customer.Username
            };
        }
    }
}
