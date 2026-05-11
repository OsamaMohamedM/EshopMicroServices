using FluentValidation;

namespace Catalog.Api.Products.CreatProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Product name is required.");
            RuleFor(p => p.Description).NotEmpty().WithMessage("Product description is required.");
            RuleFor(p => p.Price).GreaterThan(0).WithMessage("Product price must be greater than zero.");
            RuleFor(p => p.Category).NotEmpty().WithMessage("Product category is required.");
            RuleFor(p => p.ImageFile).NotEmpty().NotNull().WithMessage("Product image is required.");
        }
    }
}