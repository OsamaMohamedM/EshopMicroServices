using BuildingBlocks.CQRS;
using Catalog.Api.Models;
using Marten;

namespace Catalog.Api.Products.DeleteProductById
{
    public record DeleteProductByIdCommand(Guid Id) : ICommand<DeleteProductByIdResponse>;

    public record DeleteProductByIdResponse(bool Success);

    internal class DeleteProductByIdHandler(IDocumentSession session) : ICommandHandler<DeleteProductByIdCommand, DeleteProductByIdResponse>
    {
        public async Task<DeleteProductByIdResponse> Handle(DeleteProductByIdCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Id == Guid.Empty) throw new ArgumentException("Product ID cannot be empty", nameof(request.Id));

            var product = await session.Query<Product>()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null)
                throw new KeyNotFoundException($"Product with ID {request.Id} not found");

            session.Delete(product);
            await session.SaveChangesAsync(cancellationToken);

            return new DeleteProductByIdResponse(true);
        }
    }
}
