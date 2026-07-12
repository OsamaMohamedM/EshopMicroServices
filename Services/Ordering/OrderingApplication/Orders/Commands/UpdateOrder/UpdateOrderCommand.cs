using FluentValidation;

namespace OrderingApplication.Orders.Commands.UpdateOrder;

public record UpdateOrderCommand(OrderDto OrderDto) : ICommand<UpdateOrderResult>;

public class UpdateOrderResult(bool isSuccess)
{
    public bool IsSuccess { get; } = isSuccess;
}

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.OrderDto.Id).NotEmpty().WithMessage("Order Id is Required");
        RuleFor(x => x.OrderDto.OrderName).NotEmpty().WithMessage("Order Name is Required");
        RuleFor(x => x.OrderDto.CustomerId).NotEmpty().WithMessage("Customer Id is Required");
        RuleFor(x => x.OrderDto.ShippingAddress).NotNull().WithMessage("Shipping Address is Required");
        RuleFor(x => x.OrderDto.BillingAddress).NotNull().WithMessage("Billing Address is Required");
        RuleFor(x => x.OrderDto.Payment).NotNull().WithMessage("Payment is Required");
        RuleFor(x => x.OrderDto.Items).NotEmpty().WithMessage("Order Items are Required");
        RuleForEach(x => x.OrderDto.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product Id is Required");
            item.RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero");
            item.RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative");
        });
    }
}
