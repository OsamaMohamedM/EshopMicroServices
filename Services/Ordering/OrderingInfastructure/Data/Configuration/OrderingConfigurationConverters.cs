using OrderingDomain.ValueObject;

namespace OrderingInfastructure.Data.Configuration
{
    internal static class OrderingConfigurationConverters
    {
        public static CustomerId ToCustomerId(Guid value) => CustomerId.Create(value);

        public static OrderId ToOrderId(Guid value) => OrderId.Create(value);

        public static OrderItemId ToOrderItemId(Guid value) => OrderItemId.Create(value);

        public static ProductId ToProductId(Guid value) => ProductId.Create(value);

        public static OrderName ToOrderName(string value) => OrderName.Create(value);

        public static Quantity ToQuantity(int value) => Quantity.Create(value);

        public static Money ToMoney(decimal value) => Money.Of(value);
    }
}
