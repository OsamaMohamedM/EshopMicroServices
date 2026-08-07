using Basket.API.Dto;
using Carter;
using Mapster;
using MediatR;

namespace Basket.API.Basket.CheckOutBasket;

public record CheckOutBasketRequest(BasketDto BasketDto);
public record CheckOutBasketResponse(bool Success);

public class CheckOutEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket/checkout", async (CheckOutBasketRequest request, ISender sender) =>
        {
            var Command = request.Adapt<CheckOutBasketCommand>();
            var result = await sender.Send(Command);
            var response = result.Adapt<CheckOutBasketResponse>();
            return response;
        }).WithName("CheckOutBasket")
        .WithSummary("Check out basket")
        .WithDescription("Check out basket")
        .Produces<CheckOutBasketResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}