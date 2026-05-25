using Microsoft.EntityFrameworkCore;
using CustomerApi.Models;

namespace CustomerApi.Data
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        {
        }

        public DbSet<CustomerInfo> Customers { get; set; } = null!;
    }
}
