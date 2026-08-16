using Microsoft.EntityFrameworkCore;
using RestaurantReservation.API.DTOs;
using RestaurantReservation.Db.Context;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.API.Services;

public class ReservationService : IReservationService
{
    private readonly RestaurantReservationDbContext _context;

    public ReservationService(RestaurantReservationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReservationResponseDto>> GetAllAsync()
    {
        return await _context.Reservations
            .Select(r => new ReservationResponseDto(
                r.ReservationId, r.CustomerId, r.RestaurantId, r.TableId, r.ReservationDate, r.PartySize))
            .ToListAsync();
    }

    public async Task<ReservationResponseDto?> GetByIdAsync(int id)
    {
        var r = await _context.Reservations.FindAsync(id);
        if (r is null) return null;

        return new ReservationResponseDto(
            r.ReservationId, r.CustomerId, r.RestaurantId, r.TableId, r.ReservationDate, r.PartySize);
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

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return new ReservationResponseDto(
            reservation.ReservationId, reservation.CustomerId, reservation.RestaurantId, 
            reservation.TableId, reservation.ReservationDate, reservation.PartySize);
    }

    public async Task<bool> UpdateAsync(int id, ReservationUpdateDto dto)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation is null) return false;

        reservation.ReservationDate = dto.ReservationDate;
        reservation.PartySize = dto.PartySize;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation is null) return false;

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return true;
    }
}