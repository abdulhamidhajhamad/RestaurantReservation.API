using FluentValidation;
using RestaurantReservation.API.DTOs;
using RestaurantReservation.API.Extensions;
using RestaurantReservation.API.Services.Reservations;

namespace RestaurantReservation.API.Endpoints;

public static class ReservationEndpoints
{
    public static void MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reservations")
                       .WithTags("Reservations");

        // --- CRUD Endpoints ---

        group.MapGet("/", async (IReservationService reservationService) =>
        {
            var reservations = await reservationService.GetAllAsync();
            return Results.Ok(reservations);
        })
        .Produces(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", async (int id, IReservationService reservationService) =>
        {
            var reservation = await reservationService.GetByIdAsync(id);
            return reservation is not null 
                ? Results.Ok(reservation) 
                : Results.NotFound(new { Message = $"Reservation with ID {id} was not found." });
        })
        .ValidateId()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (
            ReservationCreateDto dto,
            IValidator<ReservationCreateDto> validator,
            IReservationService reservationService) =>
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var createdReservation = await reservationService.CreateAsync(dto);
            return Results.Created($"/api/reservations/{createdReservation.ReservationId}", createdReservation);
        })
        .RequireAuthorization(policy => policy.RequireRole("Manager"))
        .Produces(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("/{id:int}", async (
            int id, 
            ReservationUpdateDto dto, 
            IValidator<ReservationUpdateDto> validator, 
            IReservationService reservationService) =>
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var updated = await reservationService.UpdateAsync(id, dto);
            return updated 
                ? Results.NoContent() 
                : Results.NotFound(new { Message = $"Reservation with ID {id} was not found." });
        })
        .RequireAuthorization(policy => policy.RequireRole("Manager"))
        .ValidateId()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:int}", async (int id, IReservationService reservationService) =>
        {
            var deleted = await reservationService.DeleteAsync(id);
            return deleted 
                ? Results.NoContent() 
                : Results.NotFound(new { Message = $"Reservation with ID {id} was not found." });
        })
        .RequireAuthorization(policy => policy.RequireRole("Manager"))
        .ValidateId()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        // --- Additional Minimal APIs (Item 4) ---

        group.MapGet("/customer/{customerId:int}", async (int customerId, IReservationService reservationService) =>
        {
            var reservations = await reservationService.GetReservationsByCustomerAsync(customerId);
            return Results.Ok(reservations);
        })
        .ValidateId()
        .WithName("GetReservationsByCustomer")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/{reservationId:int}/orders", async (int reservationId, IReservationService reservationService) =>
        {
            var orders = await reservationService.GetOrdersByReservationAsync(reservationId);
            return orders is not null 
                ? Results.Ok(orders) 
                : Results.NotFound(new { Message = $"Reservation with ID {reservationId} was not found." });
        })
        .ValidateId()
        .WithName("GetReservationOrders")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{reservationId:int}/menu-items", async (int reservationId, IReservationService reservationService) =>
        {
            var menuItems = await reservationService.GetMenuItemsByReservationAsync(reservationId);
            return menuItems is not null 
                ? Results.Ok(menuItems) 
                : Results.NotFound(new { Message = $"Reservation with ID {reservationId} was not found." });
        })
        .ValidateId()
        .WithName("GetReservationMenuItems")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
    }
}