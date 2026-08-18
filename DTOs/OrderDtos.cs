namespace RestaurantReservation.API.DTOs;

public record OrderResponseDto(
    int OrderId,
    int ReservationId,
    int EmployeeId,
    DateTime OrderDate,
    decimal TotalAmount
);