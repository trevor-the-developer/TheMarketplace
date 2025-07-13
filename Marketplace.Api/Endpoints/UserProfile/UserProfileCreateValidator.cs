using FluentValidation;

namespace Marketplace.Api.Endpoints.UserProfile
{
    public class UserProfileCreateValidator : AbstractValidator<UserProfileCreate>
    {
        public UserProfileCreateValidator()
        {
            RuleFor(x => x.ApplicationUserId)
                .NotEmpty().WithMessage("ApplicationUserId is required");

            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage("DisplayName is required")
                .MaximumLength(100).WithMessage("DisplayName must not exceed 100 characters");

            RuleFor(x => x.Bio)
                .NotEmpty().WithMessage("Bio is required")
                .MaximumLength(1000).WithMessage("Bio must not exceed 1000 characters");

            RuleFor(x => x.SocialMedia)
                .NotEmpty().WithMessage("SocialMedia is required")
                .MaximumLength(500).WithMessage("SocialMedia must not exceed 500 characters");
        }
    }
}
