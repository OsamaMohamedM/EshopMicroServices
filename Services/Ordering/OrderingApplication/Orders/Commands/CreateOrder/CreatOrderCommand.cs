using BuildingBlocks.CQRS;
using FluentValidation;
using OrderingApplication.DTOs;

namespace OrderingApplication.Orders.Commands.CreateOrder;

public record CreatOrderCommand(OrderDto OrderDto) : ICommand<CreatOrderResult>;

public class CreatOrderResult(Guid id)
{
    public Guid Id { get; } = id;
}

public class CreatOrderCommandValidator : AbstractValidator<CreatOrderCommand>
{
    public CreatOrderCommandValidator()
    {
        RuleFor(x => x.OrderDto.OrderName).NotEmpty().WithMessage("Order Name is Required");
        RuleFor(x => x.OrderDto.CustomerId).NotNull().WithMessage("Customer Id is Required");
        RuleFor(x => x.OrderDto.Items).NotEmpty().WithMessage("Customer Id is Required");
    }
}