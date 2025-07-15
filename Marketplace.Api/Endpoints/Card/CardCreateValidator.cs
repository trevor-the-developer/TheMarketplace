using FluentValidation;

namespace Marketplace.Api.Endpoints.Card;

public class CardCreateValidator : AbstractValidator<CardCreate>
{
    public CardCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.ListingId)
            .GreaterThan(0).WithMessage("ListingId must be greater than 0");
    }
}