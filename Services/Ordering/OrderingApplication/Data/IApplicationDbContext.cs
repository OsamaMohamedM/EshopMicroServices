using Microsoft.EntityFrameworkCore;
using OrderingDomain.Models;

namespace OrderingApplication.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Order> Orders { get; }
        DbSet<Customer> Customers { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<Product> Products { get; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}