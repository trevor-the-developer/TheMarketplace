using FluentValidation;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Listing;

namespace Marketplace.Core.Validators;

public class ListingCreateValidator : AbstractValidator<ListingCreate>
{
    public ListingCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required");

        // Description is optional, no validation needed
    }
}