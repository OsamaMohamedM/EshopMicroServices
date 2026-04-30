using Carter;
using Mapster;
using MediatR;

namespace Catalog.Api.Products.UpdateProduct
{
    public record UpdateProductRequestDTO(Guid Id, string Name, string Description, decimal Price, string ImageFile, List<string> Category);
    public record UpdateProductResponseDTO(bool Success);

    public class UpdateProductAPI : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products", async (UpdateProductRequestDTO request, ISender sender) =>
            {
                var command = request.Adapt<UpdateProductCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateProductResponseDTO>();
                return Results.Ok(response);
            }
            )
            .WithName("UpdateProduct")
            .WithDescription("Updates an existing product with the specified details.")
            .WithSummary("Updates an existing product")
            .Produces<UpdateProductResponseDTO>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
