using Grpc.Core;
using RestaurantReservation.API.Protos;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Repositories;

namespace RestaurantReservation.API.Services;

public class ReservationGrpcService : ReservationGrpc.ReservationGrpcBase
{
    private readonly IReservationRepository _reservationRepository;

    public ReservationGrpcService(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public override async Task<ReservationResponse> GetReservation(GetReservationRequest request, ServerCallContext context)
    {
        var reservation = await _reservationRepository.GetReservationById(request.ReservationId);
        if (reservation is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Reservation with ID {request.ReservationId} not found"));
        }

        return MapToResponse(reservation);
    }

    public override async Task<ListReservationsResponse> ListReservations(ListReservationsRequest request, ServerCallContext context)
    {
        var reservations = await _reservationRepository.GetAllReservations();
        var response = new ListReservationsResponse();

        response.Reservations.AddRange(reservations.Select(MapToResponse));
        return response;
    }

    public override async Task<ReservationResponse> CreateReservation(CreateReservationRequest request, ServerCallContext context)
    {
        if (!DateTime.TryParse(request.ReservationDate, out var reservationDate))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid date format. Use ISO format (e.g. 2026-08-18T10:00:00Z)"));
        }

        var reservation = new Reservation
        {
            CustomerId = request.CustomerId,
            RestaurantId = request.RestaurantId,
            TableId = request.TableId,
            ReservationDate = reservationDate,
            PartySize = request.PartySize
        };

        await _reservationRepository.CreateReservation(reservation);
        return MapToResponse(reservation);
    }

    public override async Task<ReservationResponse> UpdateReservation(UpdateReservationRequest request, ServerCallContext context)
    {
        var existing = await _reservationRepository.GetReservationById(request.ReservationId);
        if (existing is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Reservation with ID {request.ReservationId} not found"));
        }

        if (!DateTime.TryParse(request.ReservationDate, out var reservationDate))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid date format"));
        }

        existing.ReservationDate = reservationDate;
        existing.PartySize = request.PartySize;

        await _reservationRepository.UpdateReservation(existing);
        return MapToResponse(existing);
    }

    public override async Task<DeleteReservationResponse> DeleteReservation(DeleteReservationRequest request, ServerCallContext context)
    {
        var existing = await _reservationRepository.GetReservationById(request.ReservationId);
        if (existing is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Reservation with ID {request.ReservationId} not found"));
        }

        await _reservationRepository.DeleteReservation(request.ReservationId);
        return new DeleteReservationResponse { Success = true };
    }

    private static ReservationResponse MapToResponse(Reservation reservation)
    {
        return new ReservationResponse
        {
            ReservationId = reservation.ReservationId,
            CustomerId = reservation.CustomerId,
            RestaurantId = reservation.RestaurantId,
            TableId = reservation.TableId,
            ReservationDate = reservation.ReservationDate.ToString("o"),
            PartySize = reservation.PartySize
        };
    }
}