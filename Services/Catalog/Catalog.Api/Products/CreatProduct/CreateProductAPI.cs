using Carter;
using Mapster;
using MediatR;

namespace Catalog.Api.Products.CreatProduct
{
    public record CreateProductRequestDTO(string Name, string Description, decimal Price, string ImageFile, List<string> Category);
    public record CreateProductResponseDTO(Guid Id);

    public class CreateProductAPI : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (CreateProductRequestDTO request, ISender sender) =>
            {
                var command = request.Adapt<CreateProductCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateProductResponseDTO>();
                return Results.Created($"/products/{response.Id}", response);
            }
            )
            .WithName("CreateProduct")
            .WithDescription("Creates a new product with the specified details and returns the created product's ID.")
            .WithSummary("Creates a new product")
            .Produces<CreateProductResponseDTO>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}