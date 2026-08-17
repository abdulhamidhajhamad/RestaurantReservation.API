using FluentValidation;
using RestaurantReservation.API.DTOs;

namespace RestaurantReservation.API.Validators;

public class ReservationUpdateDtoValidator : AbstractValidator<ReservationUpdateDto>
{
    public ReservationUpdateDtoValidator()
    {
        RuleFor(x => x.PartySize)
            .InclusiveBetween(1, 20).WithMessage("Party size must be between 1 and 20.");

        RuleFor(x => x.ReservationDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Reservation date must be in the future.");
    }
}