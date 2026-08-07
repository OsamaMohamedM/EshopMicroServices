using Carter;
using Mapster;
using MediatR;
using OrderingApplication.DTOs;
using OrderingApplication.Orders.Commands.UpdateOrder;

namespace Ordering.Api.Orders.UpdateOrder;

public record UpdateOrderRequestDto(OrderDto OrderDto);
public record UpdateOrderResponseDto(bool IsSuccess);

public class UpdateOrderApi : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/orders", async (UpdateOrderRequestDto request, ISender sender) =>
        {
            var command = request.Adapt<UpdateOrderCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<UpdateOrderResponseDto>();

            return Results.Ok(response);
        })
        .WithName("UpdateOrder")
        .WithSummary("Updates an existing order")
        .WithDescription("Update an existing Order")
        .Produces<UpdateOrderResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
