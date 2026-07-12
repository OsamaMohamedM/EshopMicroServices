using BuildingBlocks.CQRS;
using Catalog.Api.Models;
using Marten;
using Marten.Pagination;

namespace Catalog.Api.Products.GetProduct
{
    public record GetProductsQuery(int? pageNumber = 1, int? PageSize = 10) : IQuery<GetProductResult>;

    public record GetProductResult(IEnumerable<ProductDto> Data);

    public record ProductDto(Guid Id, string Name, string Description, decimal Price, string ImageFile, List<string> Category);

    public class GetProductsHandler(IDocumentSession session) : IQueryHandler<GetProductsQuery, GetProductResult>
    {
        public async Task<GetProductResult> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var products = await session.Query<Product>()
                .Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.ImageFile,
                    p.Category))
                .ToPagedListAsync(request.pageNumber ?? 1, request.PageSize ?? 10, cancellationToken);

            return new GetProductResult(products);
        }
    }
}