using Basket.API.Data.Models;
using Carter;
using Mapster;
using MediatR;

namespace Basket.API.Basket.GetBasket
{
    public record GetBasketResponse(ShoppingCart ShoppingCart);

    public class GetBasketApi() : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/basket/{userName}", async (string userName, ISender sender) =>
            {
                var result = await sender.Send(new GetBasketQuery(userName));
                var response = result.Adapt<GetBasketResponse>();
                return Results.Ok(response);
            }).WithDescription("Endpoint for getting basket items by user name")
              .WithSummary("Gets a shopping basket")
              .WithName("GetBasket")
              .Produces<GetBasketResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
