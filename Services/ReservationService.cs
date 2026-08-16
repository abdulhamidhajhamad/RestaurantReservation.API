using RestaurantReservation.API.DTOs;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Repositories;

namespace RestaurantReservation.API.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;

    public ReservationService(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<IEnumerable<ReservationResponseDto>> GetAllAsync()
    {
        var reservations = await _reservationRepository.GetAllAsync();

        return reservations.Select(r => new ReservationResponseDto(
            r.ReservationId, 
            r.CustomerId, 
            r.RestaurantId, 
            r.TableId, 
            r.ReservationDate, 
            r.PartySize));
    }

    public async Task<ReservationResponseDto?> GetByIdAsync(int id)
    {
        var r = await _reservationRepository.GetByIdAsync(id);
        if (r is null) return null;

        return new ReservationResponseDto(
            r.ReservationId, 
            r.CustomerId, 
            r.RestaurantId, 
            r.TableId, 
            r.ReservationDate, 
            r.PartySize);
    }

    public async Task<ReservationResponseDto> CreateAsync(ReservationCreateDto dto)
    {
        var reservation = new Reservation
        {
            CustomerId = dto.CustomerId,
            RestaurantId = dto.RestaurantId,
            TableId = dto.TableId,
            ReservationDate = dto.ReservationDate,
            PartySize = dto.PartySize
        };

        await _reservationRepository.CreateAsync(reservation);

        return new ReservationResponseDto(
            reservation.ReservationId, 
            reservation.CustomerId, 
            reservation.RestaurantId, 
            reservation.TableId, 
            reservation.ReservationDate, 
            reservation.PartySize);
    }

    public async Task<bool> UpdateAsync(int id, ReservationUpdateDto dto)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);
        if (reservation is null) return false;

        reservation.ReservationDate = dto.ReservationDate;
        reservation.PartySize = dto.PartySize;

        await _reservationRepository.UpdateAsync(reservation);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);
        if (reservation is null) return false;

        await _reservationRepository.DeleteAsync(id);
        return true;
    }
}