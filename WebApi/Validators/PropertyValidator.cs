using FluentValidation;
using WebApi.Domain.Dtos;

namespace WebApi.Validators
{
    public class PropertyValidator : AbstractValidator<Property_DTO>
    {
        public PropertyValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("The property name is required.")
                .MaximumLength(100).WithMessage("The property name must not exceed 100 characters.");

            RuleFor(p => p.Location)
                .NotEmpty().WithMessage("The property location is required.");

            RuleFor(p => p.PricePerNight).GreaterThan(0).WithMessage("The Price can not be less than 0");

            RuleFor(p => p.Status).NotEmpty().WithMessage("The property status is required");

            RuleFor(p => p.HostId)
                .NotEmpty().WithMessage("The HostId is required.");
        }
    }
}