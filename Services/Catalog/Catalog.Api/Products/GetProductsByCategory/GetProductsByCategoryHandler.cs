using BuildingBlocks.CQRS;
using Catalog.Api.Models;
using Marten;

namespace Catalog.Api.Products.GetProductsByCategory
{
    public record GetProductsByCategoryQuery(string Category) : IQuery<GetProductsByCategoryResult>;

    public record GetProductsByCategoryResult(IEnumerable<ProductDto> Products);

    public record ProductDto(Guid Id, string Name, string Description, decimal Price, string ImageFile, List<string> Category);

    public class GetProductsByCategoryHandler(IDocumentSession session) : IQueryHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
    {
        public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.Category)) throw new ArgumentException("Category cannot be empty", nameof(request.Category));

            var products = await session.Query<Product>()
                .Where(p => p.Category.Contains(request.Category))
                .Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.ImageFile,
                    p.Category))
                .ToListAsync(cancellationToken);

            return new GetProductsByCategoryResult(products);
        }
    }
}