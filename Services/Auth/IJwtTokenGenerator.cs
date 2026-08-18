using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.API.Services.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(Employee employee);
}