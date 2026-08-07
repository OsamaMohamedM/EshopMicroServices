using Carter;
using Mapster;
using MediatR;
using OrderingApplication.DTOs;
using OrderingApplication.Orders.Queries.GetOrdersByName;

namespace Ordering.Api.Orders.GetOrdersByName;

public record GetOrdersByNameResponseDto(IEnumerable<OrderDto> Orders);

public class GetOrdersByNameApi : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/{orderName}", async (string orderName, ISender sender) =>
        {
            var result = await sender.Send(new GetOrdersByNameQuery(orderName));
            var response = result.Adapt<GetOrdersByNameResponseDto>();

            return Results.Ok(response);
        })
        .WithName("GetOrdersByName")
        .WithSummary("Gets orders by order name")
        .WithDescription("Get Orders by OrderName")
        .Produces<GetOrdersByNameResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
