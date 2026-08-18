namespace RestaurantReservation.API.DTOs;

public record ManagerResponseDto(
    int EmployeeId,
    int RestaurantId,
    string FirstName,
    string LastName,
    string Position
);

public record EmployeeAverageOrderAmountDto(
    int EmployeeId,
    decimal AverageOrderAmount
);