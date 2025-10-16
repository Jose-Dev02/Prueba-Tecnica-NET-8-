using FluentValidation;
using Host = WebApi.Domain.Entities.Host;

namespace WebApi.Validators
{
    public class HostValidator : AbstractValidator<Host>
    {
        public HostValidator()
        {
            RuleFor(h => h.Name)
                .NotEmpty().WithMessage("The host name is required.")
                .MaximumLength(100).WithMessage("The host name must not exceed 100 characters.");
        }
    }
}