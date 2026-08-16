using RestaurantReservation.API.DTOs;

namespace RestaurantReservation.API.Services;

public interface IReservationService
{
    Task<IEnumerable<ReservationResponseDto>> GetAllAsync();
    Task<ReservationResponseDto?> GetByIdAsync(int id);
    Task<ReservationResponseDto> CreateAsync(ReservationCreateDto dto);
    Task<bool> UpdateAsync(int id, ReservationUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}