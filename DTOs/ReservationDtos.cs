namespace RestaurantReservation.API.DTOs;
public record ReservationCreateDto(
    int CustomerId, 
    int RestaurantId, 
    int TableId, 
    DateTime ReservationDate, 
    int PartySize
);

public record ReservationUpdateDto(
    DateTime ReservationDate, 
    int PartySize
);

public record ReservationResponseDto(
    int ReservationId, 
    int CustomerId, 
    int RestaurantId, 
    int TableId, 
    DateTime ReservationDate, 
    int PartySize
);