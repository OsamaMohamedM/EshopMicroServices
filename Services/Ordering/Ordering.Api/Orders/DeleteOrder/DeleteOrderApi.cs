using Carter;
using Mapster;
using MediatR;
using OrderingApplication.Orders.Commands.DeleteOrder;

namespace Ordering.Api.Orders.DeleteOrder;

public record DeleteOrderResponseDto(bool IsSuccess);

public class DeleteOrderApi : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/orders/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteOrderCommand(id));
            var response = result.Adapt<DeleteOrderResponseDto>();

            return Results.Ok(response);
        })
        .WithName("DeleteOrder")
        .WithSummary("Deletes an order")
        .WithDescription("Delete Order with Id")
        .Produces<DeleteOrderResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
