using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderingDomain.Models;

namespace OrderingInfastructure.Data.Configuration
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Id)
                .HasConversion(orderItemId => orderItemId.Value, value => OrderingConfigurationConverters.ToOrderItemId(value))
                .ValueGeneratedNever();

            builder.Property(oi => oi.OrderId)
                .HasConversion(orderId => orderId.Value, value => OrderingConfigurationConverters.ToOrderId(value))
                .IsRequired();

            builder.Property(oi => oi.ProductId)
                .HasConversion(productId => productId.Value, value => OrderingConfigurationConverters.ToProductId(value))
                .IsRequired();

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);

            builder.Property(oi => oi.Quantity)
                .HasConversion(q => q.Value, value => OrderingConfigurationConverters.ToQuantity(value))
                .IsRequired();

            builder.Property(oi => oi.UnitPrice)
                .HasConversion(money => money.Value, value => OrderingConfigurationConverters.ToMoney(value))
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        }
    }
}
