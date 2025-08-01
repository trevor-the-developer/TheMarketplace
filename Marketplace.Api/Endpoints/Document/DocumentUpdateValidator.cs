using FluentValidation;

namespace Marketplace.Api.Endpoints.Document;

public class DocumentUpdateValidator : AbstractValidator<DocumentUpdate>
{
    public DocumentUpdateValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DocumentType)
            .NotEmpty().WithMessage("DocumentType is required")
            .MaximumLength(50).WithMessage("DocumentType must not exceed 50 characters");

        RuleFor(x => x.ProductDetailId)
            .GreaterThan(0).WithMessage("ProductDetailId must be greater than 0")
            .When(x => x.ProductDetailId.HasValue);

        // Text and Bytes are optional, no constraints defined
    }
}