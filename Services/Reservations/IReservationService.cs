using RestaurantReservation.API.DTOs;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.API.Services.Reservations;

public interface IReservationService
{
    Task<IEnumerable<Reservation>> GetAllAsync();
    Task<Reservation?> GetByIdAsync(int id);
    Task<Reservation> CreateAsync(ReservationCreateDto dto);
    Task<bool> UpdateAsync(int id, ReservationUpdateDto dto);
    Task<bool> DeleteAsync(int id);

    Task<IEnumerable<Reservation>> GetReservationsByCustomerAsync(int customerId);
    Task<IEnumerable<OrderResponseDto>?> GetOrdersByReservationAsync(int reservationId); 
    Task<IEnumerable<MenuItemResponseDto>?> GetMenuItemsByReservationAsync(int reservationId);
    
}