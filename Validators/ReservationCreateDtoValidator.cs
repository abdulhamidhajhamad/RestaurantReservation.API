using FluentValidation;
using RestaurantReservation.API.DTOs;

namespace RestaurantReservation.API.Validators;

public class ReservationCreateDtoValidator : AbstractValidator<ReservationCreateDto>
{
    public ReservationCreateDtoValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("Valid Customer ID is required.");

        RuleFor(x => x.RestaurantId)
            .GreaterThan(0).WithMessage("Valid Restaurant ID is required.");

        RuleFor(x => x.PartySize)
            .InclusiveBetween(1, 20).WithMessage("Party size must be between 1 and 20.");

        RuleFor(x => x.ReservationDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Reservation date must be in the future.");
    }
}