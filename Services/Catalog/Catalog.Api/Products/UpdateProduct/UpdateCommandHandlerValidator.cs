using FluentValidation;

namespace Catalog.Api.Products.UpdateProduct
{
    public class UpdateCommandHandlerValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateCommandHandlerValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("Product ID is required.");
            RuleFor(p => p.ImageFile).NotEmpty().NotNull().WithMessage("Product image is required.");
            RuleFor(p => p.Name).NotEmpty().WithMessage("Product name is required.");
            RuleFor(p => p.Description).NotEmpty().WithMessage("Product description is required.");
            RuleFor(p => p.Price).GreaterThan(0).WithMessage("Product price must be greater than zero.");
            RuleFor(p => p.Category).NotEmpty().WithMessage("Product category is required.");
            RuleFor(p => p.Category).Must(c => c != null && c.Count > 0).WithMessage("Product category must contain at least one category.");
        }
    }
}