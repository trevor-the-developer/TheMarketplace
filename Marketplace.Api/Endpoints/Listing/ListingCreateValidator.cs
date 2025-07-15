using FluentValidation;

namespace Marketplace.Api.Endpoints.Listing;

public class ListingCreateValidator : AbstractValidator<ListingCreate>
{
    public ListingCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required");

        // Description is optional, no validation needed
    }
}