using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.API.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(Employee employee);
}