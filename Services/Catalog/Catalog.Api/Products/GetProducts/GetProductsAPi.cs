using Carter;
using Mapster;
using MediatR;

namespace Catalog.Api.Products.GetProduct
{
    public record GetProductsResponseDto(IEnumerable<ProductDto> Data);

    public class GetProductsAPi : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(
                "/products",
                async (ISender sender, [AsParameters] GetProductsQuery request) =>
                {
                    var result = await sender.Send(request);
                    var response = result.Adapt<GetProductsResponseDto>();
                    return Results.Ok(response);
                }
                ).WithName("GetProducts")
                  .WithSummary("Gets products")
                  .WithDisplayName("GetALL Products")
                  .WithDescription("Get All products in stock")
                  .Produces<GetProductsResponseDto>(StatusCodes.Status200OK)
                  .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
