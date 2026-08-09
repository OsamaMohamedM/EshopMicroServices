using Microsoft.EntityFrameworkCore;
using OrderingDomain.Models;
using OrderingDomain.ValueObject;

namespace OrderingInfastructure.Data
{
    public static class DatabaseInitialiser
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!await context.Customers.AnyAsync())
            {
                await context.Customers.AddRangeAsync(GetPreconfiguredCustomers());
                await context.SaveChangesAsync();
            }

            if (!await context.Products.AnyAsync())
            {
                await context.Products.AddRangeAsync(GetPreconfiguredProducts());
                await context.SaveChangesAsync();
            }

            if (!await context.Orders.AnyAsync())
            {
                await context.Orders.AddRangeAsync(GetPreconfiguredOrders());
                await context.SaveChangesAsync();
            }
        }

        private static IEnumerable<Customer> GetPreconfiguredCustomers()
        {
            return new List<Customer>
            {
                Customer.Create(CustomerId.Create(new Guid("5812B601-987A-4B60-B147-3855E5186E97")), "Osama Mohamed", "osama@eshop.com"),
                Customer.Create(CustomerId.Create(new Guid("8965C601-127A-4C60-B247-3855E5186E98")), "John Doe", "john@eshop.com")
            };
        }

        private static IEnumerable<Product> GetPreconfiguredProducts()
        {
            return new List<Product>
            {
                Product.Create(ProductId.Create(new Guid("5334c44e-ea39-4b2e-bb91-bc8e154625e1")), "IPhone 15", 999.00m),
                Product.Create(ProductId.Create(new Guid("6ec12674-72a7-4b11-9721-66774e12e128")), "Samsung Galaxy S24", 899.00m)
            };
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            var customerId = CustomerId.Create(new Guid("5812B601-987A-4B60-B147-3855E5186E97"));
            var product1 = ProductId.Create(new Guid("5334c44e-ea39-4b2e-bb91-bc8e154625e1"));
            var product2 = ProductId.Create(new Guid("6ec12674-72a7-4b11-9721-66774e12e128"));

            var order = Order.Create(
                OrderId.Create(new Guid("a75c3d4f-4d6d-4952-97bb-23945f3cbb01")),
                customerId,
                OrderName.Create("Order #1001"),
                Address.Create("Osama", "Mohamed", "osama@eshop.com", "123 Main St", "Egypt", "Cairo", "19178"),
                Address.Create("Osama", "Mohamed", "osama@eshop.com", "123 Main St", "Egypt", "Cairo", "19178"),
                Payment.Of("Osama Mohamed", "1234567890123456", "12/28", "123", 1)
            );

            order.Add(product1, 1, 999.00m);
            order.Add(product2, 2, 899.00m);

            return new List<Order> { order };
        }
    }
}