namespace RestaurantReservation.API.Services.Interfaces;
public interface IAuth
{
    Task<string?> LoginAsync(string employeeName);
}