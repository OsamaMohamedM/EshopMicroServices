using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderingDomain.Enums;
using OrderingDomain.Models;
using OrderingDomain.ValueObject;

namespace OrderingInfastructure.Data.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .HasConversion(orderId => orderId.Value, value => OrderingConfigurationConverters.ToOrderId(value))
                .ValueGeneratedNever();

            builder.Property(o => o.CustomerId)
                .HasConversion(customerId => customerId.Value, value => OrderingConfigurationConverters.ToCustomerId(value))
                .IsRequired();

            builder.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(o => o.CustomerId);

            builder.Property(o => o.OrderName)
                .HasConversion(orderName => orderName.Value, value => OrderingConfigurationConverters.ToOrderName(value))
                .HasMaxLength(100)
                .IsRequired();

            builder.OwnsOne(o => o.ShippingAddress, address =>
            {
                address.Property(a => a.FirstName).HasMaxLength(50).IsRequired();
                address.Property(a => a.LastName).HasMaxLength(50).IsRequired();
                address.Property(a => a.EmailAddress).HasMaxLength(50);
                address.Property(a => a.AddressLine).HasMaxLength(180).IsRequired();
                address.Property(a => a.Country).HasMaxLength(50).IsRequired();
            });

            builder.OwnsOne(o => o.BillingAddress, address =>
            {
                address.Property(a => a.FirstName).HasMaxLength(50).IsRequired();
                address.Property(a => a.LastName).HasMaxLength(50).IsRequired();
                address.Property(a => a.EmailAddress).HasMaxLength(50);
                address.Property(a => a.AddressLine).HasMaxLength(180).IsRequired();
                address.Property(a => a.Country).HasMaxLength(50).IsRequired();
            });

            builder.OwnsOne(o => o.Payment, payment =>
            {
                payment.Property(p => p.CardName).HasMaxLength(50).IsRequired();
                payment.Property(p => p.CardNumber).HasMaxLength(24).IsRequired();
                payment.Property(p => p.Expiration).HasMaxLength(10).IsRequired();
                payment.Property(p => p.Cvv).HasMaxLength(3).IsRequired();
                payment.Property(p => p.PaymentMethod).IsRequired();
            });

            builder.Property(o => o.Status)
                .HasDefaultValue(OrderStatus.Pending)
                .HasConversion<string>();

            builder.Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId);

            builder.Ignore(o => o.DomainEvents);
        }
    }
}
