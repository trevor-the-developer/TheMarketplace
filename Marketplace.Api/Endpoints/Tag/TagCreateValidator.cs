using FluentValidation;

namespace Marketplace.Api.Endpoints.Tag;

public class TagCreateValidator : AbstractValidator<TagCreate>
{
    public TagCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // IsEnabled is optional, no validation needed
    }
}