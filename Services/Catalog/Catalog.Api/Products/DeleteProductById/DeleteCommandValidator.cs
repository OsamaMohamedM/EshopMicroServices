using FluentValidation;

namespace Catalog.Api.Products.DeleteProductById
{
    public class DeleteProductByIdCommandValidator : AbstractValidator<DeleteProductByIdCommand>
    {
        public DeleteProductByIdCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ProductId is required.");
        }
    }
}