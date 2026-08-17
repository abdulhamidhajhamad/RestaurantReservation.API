namespace RestaurantReservation.API.Services.Auth;
public interface IAuth
{
    Task<string?> LoginAsync(string employeeName);
}