using FluentValidation;

namespace Basket.API.Basket.DeleteBasket
{
    public class DeleteBasketValidator : AbstractValidator<DeleteBasketCommand>
    {
        public DeleteBasketValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .MaximumLength(100).WithMessage("UserName cannot exceed 100 characters.");
        }
    }
}