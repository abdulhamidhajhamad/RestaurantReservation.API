namespace RestaurantReservation.API.DTOs;

public record LoginRequestDto(string EmployeeName);
public record LoginResponseDto(string Token);