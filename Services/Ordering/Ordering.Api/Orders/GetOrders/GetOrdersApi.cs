using Carter;
using Mapster;
using MediatR;
using OrderingApplication.Common.Pagination;
using OrderingApplication.DTOs;
using OrderingApplication.Orders.Queries.GetOrders;

namespace Ordering.Api.Orders.GetOrders;

public record GetOrdersResponseDto(PaginatedResult<OrderDto> Orders);

public class GetOrdersApi : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders", async (ISender sender, [AsParameters] GetOrdersQuery request) =>
        {
            var result = await sender.Send(request);
            var response = result.Adapt<GetOrdersResponseDto>();

            return Results.Ok(response);
        })
        .WithName("GetOrders")
        .WithSummary("Gets orders")
        .WithDescription("Get Orders w/ Pagination")
        .Produces<GetOrdersResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
