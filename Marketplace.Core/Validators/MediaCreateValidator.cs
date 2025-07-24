using FluentValidation;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Media;

namespace Marketplace.Core.Validators;

public class MediaCreateValidator : AbstractValidator<MediaCreate>
{
    public MediaCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required");

        // Optional fields - no length constraints defined in MediaConfiguration
        // Description, FilePath, DirectoryPath, MediaType are all optional

        RuleFor(x => x.ProductDetailId)
            .GreaterThan(0).WithMessage("ProductDetailId must be greater than 0")
            .When(x => x.ProductDetailId.HasValue);
    }
}