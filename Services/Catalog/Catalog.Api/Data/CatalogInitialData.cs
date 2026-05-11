using Catalog.Api.Models;
using Marten;
using Marten.Schema;

namespace Catalog.Api.Data
{
    public class CatalogInitialData : IInitialData
    {
        public async Task Populate(IDocumentStore store, CancellationToken cancellation)
        {
            using var session = store.LightweightSession();
            if (await session.Query<Product>().AnyAsync())
                return;
            session.Store<Product>(GetPreConfigurationProducts());
            await session.SaveChangesAsync(cancellation);
        }

        private static IEnumerable<Product> GetPreConfigurationProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Product 1",
                    Description = "Description for Product 1",
                    Price = 10.99m,
                    Category =new List<string>(){ "Category A" }
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Product 2",
                    Description = "Description for Product 2",
                    Price = 19.99m,
                    Category =new List<string>(){ "Category B" }
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Product 3",
                    Description = "Description for Product 3",
                    Price = 5.99m,
                    Category = new List<string>(){"Category A" }
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Product 4",
                    Description = "Description for Product 4",
                    Price = 15.99m,
                    Category = new List<string>(){"Category C" }
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Product 5",
                    Description = "Description for Product 5",
                    Category = new List<string>(){"Category B" },
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Product 6",
                    Description = "Description for Product 6",
                    Category= new List<string>(){"Category A" },
                }
            };
        }
    }
}