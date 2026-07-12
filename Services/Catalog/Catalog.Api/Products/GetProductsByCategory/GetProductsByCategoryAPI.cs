using Carter;
using Mapster;
using MediatR;

namespace Catalog.Api.Products.GetProductsByCategory
{
    public record GetProductsByCategoryResponseDto(List<ProductDto> Data);

    public class GetProductsByCategoryAPI : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            TypeAdapterConfig<GetProductsByCategoryResult, GetProductsByCategoryResponseDto>
                .NewConfig()
                .Map(dest => dest.Data, src => src.Products.ToList())
                .Compile();

            app.MapGet(
                "/products/category/{category}",
                async (string category, ISender sender) =>
                {
                    var result = await sender.Send(new GetProductsByCategoryQuery(category));
                    var response = result.Adapt<GetProductsByCategoryResponseDto>();
                    return Results.Ok(response);
                }
                ).WithName("GetProductsByCategory")
                  .WithSummary("Gets products by category")
                  .WithDisplayName("GetProductsByCategory")
                  .WithDescription("Get all products by category")
                  .Produces<GetProductsByCategoryResponseDto>(StatusCodes.Status200OK)
                  .ProducesProblem(StatusCodes.Status404NotFound)
                  .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
