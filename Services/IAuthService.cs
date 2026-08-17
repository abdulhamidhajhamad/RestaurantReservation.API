namespace RestaurantReservation.API.Services;
public interface IAuthService
{
    Task<string?> LoginAsync(string employeeName);
}