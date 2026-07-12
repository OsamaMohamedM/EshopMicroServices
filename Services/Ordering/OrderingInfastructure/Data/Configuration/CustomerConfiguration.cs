using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderingDomain.Models;

namespace OrderingInfastructure.Data.Configuration
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(customer => customer.Id);

            builder.Property(customer => customer.Id)
                .HasConversion(customerId => customerId.Value, value => OrderingConfigurationConverters.ToCustomerId(value))
                .ValueGeneratedNever();

            builder.Property(customer => customer.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(customer => customer.Email)
                .HasMaxLength(256)
                .IsRequired();
        }
    }
}
