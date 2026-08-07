using Carter;
using Mapster;
using MediatR;
using OrderingApplication.DTOs;
using OrderingApplication.Orders.Queries.GetOrdersByCustomer;

namespace Ordering.Api.Orders.GetOrdersByCustomer;

public record GetOrdersByCustomerResponseDto(IEnumerable<OrderDto> Orders);

public class GetOrdersByCustomerApi : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/customer/{customerId}", async (Guid customerId, ISender sender) =>
        {
            var result = await sender.Send(new GetOrdersByCustomerQuery(customerId));
            var response = result.Adapt<GetOrdersByCustomerResponseDto>();

            return Results.Ok(response);
        })
        .WithName("GetOrdersByCustomer")
        .WithSummary("Gets orders by customer")
        .WithDescription("Get Orders by Customer")
        .Produces<GetOrdersByCustomerResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
