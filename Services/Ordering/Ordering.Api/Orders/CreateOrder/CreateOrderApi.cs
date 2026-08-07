using Carter;
using Mapster;
using MediatR;
using OrderingApplication.DTOs;
using OrderingApplication.Orders.Commands.CreateOrder;

namespace Ordering.Api.Orders.CreateOrder;

public record CreateOrderRequestDto(OrderDto OrderDto);
public record CreateOrderResponseDto(Guid Id);

public class CreateOrderApi : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/orders", async (CreateOrderRequestDto request, ISender sender) =>
        {
            var command = request.Adapt<CreatOrderCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CreateOrderResponseDto>();

            return Results.Created($"/orders/{response.Id}", response);
        })
        .WithName("CreateOrder")
        .WithSummary("Creates a new order")
        .WithDescription("Create a new Order")
        .Produces<CreateOrderResponseDto>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
