using FluentValidation;
using Marketplace.Core.Models.Media;

namespace Marketplace.Core.Validators;

public class MediaUpdateValidator : AbstractValidator<MediaUpdate>
{
    public MediaUpdateValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.FilePath)
            .NotEmpty().WithMessage("FilePath is required")
            .MaximumLength(500).WithMessage("FilePath must not exceed 500 characters");
        
        RuleFor(x => x.DirectoryPath)
            .NotEmpty().WithMessage("DirectoryPath is required")
            .MaximumLength(500).WithMessage("DirectoryPath must not exceed 500 characters");
        
        RuleFor(x => x.MediaType).MaximumLength(100)
            .WithMessage("MediaType must not exceed 100 characters");
    }
}