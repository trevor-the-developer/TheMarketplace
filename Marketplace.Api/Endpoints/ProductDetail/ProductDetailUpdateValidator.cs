using FluentValidation;

namespace Marketplace.Api.Endpoints.ProductDetail
{
    public class ProductDetailUpdateValidator : AbstractValidator<ProductDetailUpdate>
    {
        public ProductDetailUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ProductId must be greater than 0")
                .When(x => x.ProductId.HasValue);
        }
    }
}
