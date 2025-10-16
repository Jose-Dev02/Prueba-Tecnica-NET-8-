using FluentValidation;
using WebApi.Domain.RequestObjects;

namespace WebApi.Validators
{
    public class UserValidator : AbstractValidator<UserRequest>
    {
        public UserValidator()
        {
            RuleFor(u => u.Username)
                .NotEmpty().WithMessage("The username is required.")
                .MaximumLength(50).WithMessage("The username must not exceed 50 characters.");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("The password hash is required.");
        }
    }
}