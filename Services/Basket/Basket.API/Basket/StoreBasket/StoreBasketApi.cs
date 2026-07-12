using Basket.API.Data.Models;
using Carter;
using Mapster;
using MediatR;

namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketRequest(string UserName, List<ShoppingCartItem> Items);
    public record StoreBasketResponse(string UserName);

    public class StoreBasketApi : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/basket", async (StoreBasketRequest request, ISender sender) =>
             {
                 var command = request.Adapt<StoreBasketCommand>();
                 var result = await sender.Send(command);
                 var response = result.Adapt<StoreBasketResponse>();
                 if (result.Success)
                 {
                     return Results.Created($"basket/{response.UserName}", response);
                 }
                 else
                 {
                     return Results.BadRequest("Failed to store the basket.");
                 }
             }).WithDescription("Endpoint for storing basket items by user name")
               .WithSummary("Stores a shopping basket")
               .WithName("StoreBasket")
               .Produces<StoreBasketResponse>(StatusCodes.Status201Created)
               .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
