using RestaurantReservation.API.DTOs;
using RestaurantReservation.API.Services;

namespace RestaurantReservation.API.Endpoints;

public static class ReservationEndpoints
{
    public static void MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reservations")
                       .WithTags("Reservations");

        group.MapGet("/", async (IReservationService reservationService) =>
        {
            var reservations = await reservationService.GetAllAsync();
            return Results.Ok(reservations);
        });

        group.MapGet("/{id:int}", async (int id, IReservationService reservationService) =>
        {
            var reservation = await reservationService.GetByIdAsync(id);
            return reservation is not null 
                ? Results.Ok(reservation) 
                : Results.NotFound(new { Message = $"Reservation with ID {id} was not found." });
        });

        group.MapPost("/", async (ReservationCreateDto dto, IReservationService reservationService) =>
        {
            var createdReservation = await reservationService.CreateAsync(dto);
            return Results.Created($"/api/reservations/{createdReservation.ReservationId}", createdReservation);
        });

        group.MapPut("/{id:int}", async (int id, ReservationUpdateDto dto, IReservationService reservationService) =>
        {
            var updated = await reservationService.UpdateAsync(id, dto);
            return updated 
                ? Results.NoContent() 
                : Results.NotFound(new { Message = $"Reservation with ID {id} was not found." });
        });

        group.MapDelete("/{id:int}", async (int id, IReservationService reservationService) =>
        {
            var deleted = await reservationService.DeleteAsync(id);
            return deleted 
                ? Results.NoContent() 
                : Results.NotFound(new { Message = $"Reservation with ID {id} was not found." });
        });
    }
}