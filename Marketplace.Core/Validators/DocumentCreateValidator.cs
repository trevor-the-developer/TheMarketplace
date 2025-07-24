using FluentValidation;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Document;

namespace Marketplace.Core.Validators;

public class DocumentCreateValidator : AbstractValidator<DocumentCreate>
{
    public DocumentCreateValidator()
    {
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