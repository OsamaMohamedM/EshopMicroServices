using Carter;
using Mapster;
using MediatR;

namespace Catalog.Api.Products.GetProductById
{
    public record GetProductByIdResponseDto(ProductDto Data);

    public class GetProductByIdAPI : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(
                "/products/{id}",
                async (Guid id, ISender sender) =>
                {
                    var result = await sender.Send(new GetProductByIdQuery(id));
                    var response = result.Adapt<GetProductByIdResponseDto>();
                    return Results.Ok(response);
                }
                ).WithName("GetProductById")
                  .WithSummary("Gets a product by id")
                  .WithDisplayName("GetProductById")
                  .WithDescription("Get product by Id")
                  .Produces<GetProductByIdResponseDto>(StatusCodes.Status200OK)
                  .ProducesProblem(StatusCodes.Status404NotFound)
                  .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
