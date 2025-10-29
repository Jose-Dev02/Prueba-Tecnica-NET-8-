using FluentValidation;
using WebApi.Domain.Entities;

namespace WebApi.Validators
{
    public class PropertyValidator : AbstractValidator<Property>
    {
        public PropertyValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("The property name is required.")
                .MaximumLength(100).WithMessage("The property name must not exceed 100 characters.");

            RuleFor(p => p.Location)
                .NotEmpty().WithMessage("The property location is required.");

            RuleFor(p => p.HostId)
                .NotEmpty().WithMessage("The HostId is required.");
        }
    }
}