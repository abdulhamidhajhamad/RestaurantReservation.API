namespace RestaurantReservation.API.DTOs;

public record MenuItemResponseDto(
    int MenuItemId,
    int RestaurantId,
    string Name,
    string Description,
    decimal Price
);

public record OrderItemResponseDto(
    int OrderItemId,
    int Quantity,
    MenuItemResponseDto MenuItem
);

public record OrderWithDetailsResponseDto(
    int OrderId,
    int ReservationId,
    int EmployeeId,
    DateTime OrderDate,
    decimal TotalAmount,
    List<OrderItemResponseDto> OrderItems
);