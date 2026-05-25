using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CustomerApi.Migrations
{
    [DbContext(typeof(CustomerApi.Data.CustomerContext))]
    partial class CustomerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("CustomerApi.Models.CustomerInfo", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");

                b.Property<string>("Name").IsRequired().HasColumnType("TEXT");

                b.Property<string>("Email").HasColumnType("TEXT");

                b.Property<string>("PhoneNumber").HasColumnType("TEXT");

                b.Property<string>("Username").IsRequired().HasColumnType("TEXT");

                b.Property<string>("PasswordHash").IsRequired().HasColumnType("TEXT");

                b.Property<string>("PasswordSalt").IsRequired().HasColumnType("TEXT");

                b.HasKey("Id");

                b.ToTable("Customers");
            });
        }
    }
}
