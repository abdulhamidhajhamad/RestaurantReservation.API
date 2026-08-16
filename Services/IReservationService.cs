namespace RestaurantReservation.API.DTOs;
namespace RestaurantReservation.API.Services;
public class IReservationService
{
    Task<IEnumerable<ReservationResponseDto>> GetAllAsync();
    Task<ReservationResponseDto?>GetByIdAsync(int reservationId);
    Task<ReservationResponseDto> CreateAsync(ReservationCreateDto dto)
    Task<bool> UpateAsync(int id,ReservationUpdateDto dto)
    Task<bool> DeleteAsync(int id)
    
}