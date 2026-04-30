using Carter;
using Mapster;
using MediatR;

namespace Catalog.Api.Products.DeleteProductById
{
    public record DeleteProductByIdResponseDTO(bool Success);

    public class DeleteProductByIdAPI : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/products/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteProductByIdCommand(id));
                var response = result.Adapt<DeleteProductByIdResponseDTO>();
                return Results.Ok(response);
            }
            )
            .WithName("DeleteProductById")
            .WithDescription("Deletes a product by its ID.")
            .WithSummary("Deletes a product")
            .Produces<DeleteProductByIdResponseDTO>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
