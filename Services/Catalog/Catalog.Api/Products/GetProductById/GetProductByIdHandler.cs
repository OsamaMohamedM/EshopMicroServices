using BuildingBlocks.CQRS;
using Catalog.Api.Models;
using Marten;

namespace Catalog.Api.Products.GetProductById
{
    public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;

    public record GetProductByIdResult(ProductDto Product);
    public record ProductDto(Guid Id, string Name, string Description, decimal Price, string ImageFile, List<string> Category);

    public class GetProductByIdHandler(IDocumentSession session) : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Id == Guid.Empty) throw new ArgumentException("Product ID cannot be empty", nameof(request.Id));

            
            var product = await session.Query<Product>()
                .Where(p => p.Id == request.Id)
                .Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.ImageFile,
                    p.Category))
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
                throw new KeyNotFoundException($"Product with ID {request.Id} not found");

            return new GetProductByIdResult(product);
        }
    }
}