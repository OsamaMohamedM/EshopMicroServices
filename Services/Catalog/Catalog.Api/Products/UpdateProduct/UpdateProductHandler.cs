using BuildingBlocks.CQRS;
using Catalog.Api.Exception;
using Catalog.Api.Models;
using Marten;

namespace Catalog.Api.Products.UpdateProduct
{
    public record UpdateProductCommand(Guid Id, string Name, string Description, decimal Price, string ImageFile, List<string> Category) : ICommand<UpdateProductResponse>;

    public record UpdateProductResponse(bool Success);

    internal class UpdateProductHandler(IDocumentSession session) : ICommandHandler<UpdateProductCommand, UpdateProductResponse>
    {
        public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Id == Guid.Empty) throw new ArgumentException("Product ID cannot be empty", nameof(request.Id));

            var product = await session.LoadAsync<Product>(request.Id, cancellationToken);

            if (product == null)
                throw new ProductNotFountException(request.Id);

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.ImageFile = request.ImageFile;
            product.Category = request.Category;

            session.Update(product);
            await session.SaveChangesAsync(cancellationToken);

            return new UpdateProductResponse(true);
        }
    }
}