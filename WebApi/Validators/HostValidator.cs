using FluentValidation;
using WebApi.Domain.Dtos;

namespace WebApi.Validators
{
    public class HostValidator : AbstractValidator<Host_DTO>
    {
        public HostValidator()
        {
            RuleFor(h => h.FullName)
                .NotEmpty().WithMessage("The host full name is required.")
                .MaximumLength(100).WithMessage("The host full name must not exceed 100 characters.");

            RuleFor(h => h.Email)
                .NotEmpty().WithMessage("The host email is required.").EmailAddress();

            RuleFor(h => h.Phone)
                .NotEmpty().WithMessage("The host phone is required.");

        }
    }
}