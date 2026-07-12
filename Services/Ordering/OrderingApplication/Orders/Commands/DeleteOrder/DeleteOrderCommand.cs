using FluentValidation;

namespace OrderingApplication.Orders.Commands.DeleteOrder;

public record DeleteOrderCommand(Guid OrderId) : ICommand<DeleteOrderResult>;

public class DeleteOrderResult(bool isSuccess)
{
    public bool IsSuccess { get; } = isSuccess;
}

public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("Order Id is Required");
    }
}
