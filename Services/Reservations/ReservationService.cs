using RestaurantReservation.API.DTOs;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Repositories;

namespace RestaurantReservation.API.Services.Reservations;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;

    public ReservationService(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync()
    {
        return await _reservationRepository.GetAllReservations();
    }

    public async Task<Reservation?> GetByIdAsync(int id)
    {
        return await _reservationRepository.GetReservationById(id);
    }

    public async Task<Reservation> CreateAsync(ReservationCreateDto dto)
    {
        var reservation = new Reservation
        {
            CustomerId = dto.CustomerId,
            RestaurantId = dto.RestaurantId,
            TableId = dto.TableId,
            ReservationDate = dto.ReservationDate,
            PartySize = dto.PartySize
        };

        await _reservationRepository.CreateReservation(reservation);
        return reservation;
    }

    public async Task<bool> UpdateAsync(int id, ReservationUpdateDto dto)
    {
        var existing = await _reservationRepository.GetReservationById(id);
        if (existing is null) return false;

        existing.ReservationDate = dto.ReservationDate;
        existing.PartySize = dto.PartySize;

        await _reservationRepository.UpdateReservation(existing);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _reservationRepository.GetReservationById(id);
        if (existing is null) return false;

        await _reservationRepository.DeleteReservation(id);
        return true;
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByCustomerAsync(int customerId)
    {
        return await _reservationRepository.GetReservationsByCustomer(customerId);
    }

    public async Task<IEnumerable<OrderResponseDto>?> GetOrdersByReservationAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetReservationById(reservationId);
        if (reservation is null) return null;

        var orders = await _reservationRepository.ListOrdersAndMenuItems(reservationId);
    
        return orders.Select(o => new OrderResponseDto(
            o.OrderId,
            o.ReservationId,
            o.EmployeeId,
            o.OrderDate,
            o.TotalAmount
        ));
    }

    public async Task<IEnumerable<MenuItemResponseDto>?> GetMenuItemsByReservationAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetReservationById(reservationId);
        if (reservation is null) return null;

        var orders = await _reservationRepository.ListOrdersAndMenuItems(reservationId);

        return orders
            .SelectMany(o => o.OrderItems)
            .Select(oi => oi.MenuItem)
            .Where(mi => mi is not null)
            .Select(mi => new MenuItemResponseDto(
                mi.ItemId,
                mi.RestaurantId,
                mi.Name,
                mi.Description,
                mi.Price
            ));
    }
}