using FluentValidation;
using Host = WebApi.Domain.Entities.Host;

namespace WebApi.Validators
{
    public class HostValidator : AbstractValidator<Host>
    {
        public HostValidator()
        {
            RuleFor(h => h.FullName)
                .NotEmpty().WithMessage("The host full name is required.")
                .MaximumLength(100).WithMessage("The host full name must not exceed 100 characters.");
        }
    }
}