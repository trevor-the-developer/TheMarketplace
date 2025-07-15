using FluentValidation;

namespace Marketplace.Api.Endpoints.Product;

public class ProductUpdateValidator : AbstractValidator<ProductUpdate>
{
    public ProductUpdateValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ProductType)
            .MaximumLength(50).WithMessage("ProductType must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.ProductType));

        RuleFor(x => x.Category)
            .MaximumLength(50).WithMessage("Category must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Category));

        RuleFor(x => x.CardId)
            .GreaterThan(0).WithMessage("CardId must be greater than 0")
            .When(x => x.CardId.HasValue);

        RuleFor(x => x.ProductDetailId)
            .GreaterThan(0).WithMessage("ProductDetailId must be greater than 0")
            .When(x => x.ProductDetailId.HasValue);
    }
}