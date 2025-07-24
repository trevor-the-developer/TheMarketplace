using FluentValidation;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Card;

namespace Marketplace.Core.Validators;

public class CardUpdateValidator : AbstractValidator<CardUpdate>
{
    public CardUpdateValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}