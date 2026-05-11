using Carter;
using Mapster;
using MediatR;

namespace Catalog.Api.Products.GetProduct
{
    public record GetProductsResponseDto(List<ProductDto> Data);

    public class GetProductsAPi : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(
                "/products",
                async (ISender sender, [AsParameters] GetProductsQuery request) =>
                {
                    var query = request.Adapt<GetProductsQuery>();
                    var result = await sender.Send(new GetProductsQuery());
                    var response = result.Adapt<GetProductsResponseDto>();
                    return Results.Ok(response);
                }
                ).WithDisplayName("GetALL Products")
                  .WithDescription("Get All products in stock")
                  .Produces<GetProductsResponseDto>(StatusCodes.Status200OK)
                  .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}