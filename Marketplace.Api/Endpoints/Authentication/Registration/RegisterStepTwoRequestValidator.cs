using FluentValidation;

namespace Marketplace.Api.Endpoints.Authentication.Registration
{
    public class RegisterStepTwoRequestValidator : AbstractValidator<RegisterStepTwoRequest>
    {
        public RegisterStepTwoRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(254).WithMessage("Email must not exceed 254 characters");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required");
        }
    }
}
