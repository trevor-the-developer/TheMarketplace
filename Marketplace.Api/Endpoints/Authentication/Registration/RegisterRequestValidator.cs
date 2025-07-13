using FluentValidation;

namespace Marketplace.Api.Endpoints.Authentication.Registration
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName is required")
                .MaximumLength(100).WithMessage("FirstName must not exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required")
                .MaximumLength(100).WithMessage("LastName must not exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(254).WithMessage("Email must not exceed 254 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters");

            RuleFor(x => x.DateOfBirth)
                .Must(BeAValidAge).WithMessage("Must be at least 13 years old")
                .Must(BeNotInFuture).WithMessage("Date of birth cannot be in the future");

            // Role validation is handled by enum constraints
        }

        private bool BeAValidAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age)) age--;
            return age >= 13;
        }

        private bool BeNotInFuture(DateTime dateOfBirth)
        {
            return dateOfBirth <= DateTime.Today;
        }
    }
}
