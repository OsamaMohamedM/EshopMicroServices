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
            
            TypeAdapterConfig<GetProductResult, GetProductsResponseDto>
                .NewConfig()
                .Map(dest => dest.Data, src => src.Products.ToList())
                .Compile();

            app.MapGet(
                "/products",
                async (ISender sender) =>
                {
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