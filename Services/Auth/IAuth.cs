namespace RestaurantReservation.API.Services.Auth;

public interface IAuth
{
    Task<string?> LoginAsync(string employeeName);
    Task<bool> LogoutAsync(string? jti, string? expString);
    Task<bool> LogoutAllDevicesAsync(string? userIdString);
}