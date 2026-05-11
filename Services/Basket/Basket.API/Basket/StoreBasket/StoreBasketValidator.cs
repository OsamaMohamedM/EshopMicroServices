using FluentValidation;

namespace Basket.API.Basket.StoreBasket
{
    public class StoreBasketValidator : AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketValidator()
        {
            RuleFor(x => x.Cart.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .MaximumLength(100).WithMessage("UserName cannot exceed 100 characters.");
            RuleFor(x => x.Cart.Items)
                .NotNull().WithMessage("Items cannot be null.")
                .Must(items => items.Count > 0).WithMessage("At least one item is required in the basket.");
        }
    }
}