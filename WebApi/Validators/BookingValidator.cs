using FluentValidation;
using WebApi.Domain.Dtos;

namespace WebApi.Validators
{
    public class BookingValidator : AbstractValidator<Booking_DTO>
    {
        public BookingValidator()
        {
            RuleFor(b => b.PropertyId)
                .NotEmpty().WithMessage("The PropertyId is required.");

            RuleFor(b => b.CheckIn)
                .NotEmpty().WithMessage("The CheckIn date is required.")
                .LessThan(b => b.CheckOut).WithMessage("CheckIn must be before CheckOut.");

            RuleFor(b => b.CheckOut)
                .NotEmpty().WithMessage("The CheckOut date is required.");

            //RuleFor(b => b.TotalPrice)
            //    .GreaterThanOrEqualTo(0).WithMessage("The TotalPrice must be greater than or equal to 0.");
        }
    }
}