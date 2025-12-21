using FluentValidation;
using Marketplace.Core.Models.Media;

namespace Marketplace.Core.Validators;

public class MediaCreateWithFileValidator : AbstractValidator<MediaCreateWithFile>
{
    public MediaCreateWithFileValidator()
    {
        // Include all base MediaCreate validation rules
        Include(new MediaCreateValidator());

        // Add file-specific validation rules
        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("File is required for upload");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File name is required");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Content type is required");
    }
}
