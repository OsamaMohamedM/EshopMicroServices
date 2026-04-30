using BuildingBlocks.CQRS;
using Catalog.Api.Models;
using Marten;

namespace Catalog.Api.Products.CreatProduct
{
    public record CreateProductCommand(string Name, string Description, decimal Price, string ImageFile, List<string> Category) : ICommand<CreateProductResponse>;

    public record CreateProductResponse(Guid Id);

    internal class CreateProductHandler(IDocumentSession session) : ICommandHandler<CreateProductCommand, CreateProductResponse>
    {
        public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ImageFile = request.ImageFile,
                Category = request.Category
            };
            session.Store(product);
            await session.SaveChangesAsync(cancellationToken);

            return new CreateProductResponse(product.Id);
        }
    }
}