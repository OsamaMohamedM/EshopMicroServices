using OrderingDomain.Abestrations;
using OrderingDomain.ValueObject;

namespace OrderingDomain.Models
{
    public class OrderItem : Entity<OrderItemId>
    {
        public static OrderItem Create(OrderId orderId, ProductId productId, Quantity quantity, Money price)
        {
            return new OrderItem
            {
                Id = OrderItemId.Create(Guid.NewGuid()),
                ProductId = productId,
                Quantity = quantity,
                OrderId = orderId,
                UnitPrice = price
            };
        }

        public ProductId ProductId { get; private set; }
        public OrderId OrderId { get; private set; }
        public Quantity Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
    }
}