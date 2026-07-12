using OrderingDomain.Abestrations;
using OrderingDomain.Enums;
using OrderingDomain.Evenets;
using OrderingDomain.ValueObject;

namespace OrderingDomain.Models
{
    public class Order : Aggregate<OrderId>
    {
        private List<OrderItem> _items { get; } = new List<OrderItem>();
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

        public CustomerId CustomerId { get; private set; }
        public OrderName OrderName { get; private set; } = default!;

        public Address ShippingAddress { get; private set; } = default!;
        public Address BillingAddress { get; private set; } = default!;

        public Payment Payment { get; private set; } = default!;
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;

        public decimal TotalPrice
        {
            get => _items.Sum(i => i.UnitPrice.Value * i.Quantity.Value);
            private set;
        }

        public static Order Create(
            OrderId orderId,
            CustomerId customerId,
            OrderName orderName,
            Address shippingAddress,
            Address billingAddress,
            Payment payment)
        {
            var order = new Order
            {
                Id = orderId,
                CustomerId = customerId,
                OrderName = orderName,
                ShippingAddress = shippingAddress,
                BillingAddress = billingAddress,
                Payment = payment
            };
            order.AddDomainEvent(new OrderCreatedEvent(order));
            return order;
        }

        public void Update(OrderId orderId,
            CustomerId customerId,
            OrderName orderName,
            Address shippingAddress,
            Address billingAddress,
            Payment payment)
        {
            Id = orderId;
            CustomerId = customerId;
            OrderName = orderName;
            ShippingAddress = shippingAddress;
            Payment = payment;
            BillingAddress = billingAddress;
            AddDomainEvent(new OrderUpdatedEvent(this));
        }

        public void Add(ProductId productId, int quantity, decimal price)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));
            ArgumentOutOfRangeException.ThrowIfNegative(price, nameof(price));
            var orderItem = OrderItem.Create(Id, productId, Quantity.Create(quantity), Money.Of(price));
            _items.Add(orderItem);
        }

        public void Remove(ProductId productId)
        {
            var orderItem = _items.FirstOrDefault(i => i.ProductId == productId);
            if (orderItem is not null)
            {
                _items.Remove(orderItem);
            }
        }
    }
}