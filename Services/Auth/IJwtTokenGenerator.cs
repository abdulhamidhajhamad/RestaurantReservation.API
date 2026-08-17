using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.API.Services.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Employee employee);
}