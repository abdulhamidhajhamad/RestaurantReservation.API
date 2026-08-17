using RestaurantReservation.API.DTOs;
using RestaurantReservation.API.Services.Auth;

namespace RestaurantReservation.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (LoginRequestDto dto, IAuth authService) =>
            {
                var token = await authService.LoginAsync(dto.EmployeeName);

                if (token is null)
                {
                    return Results.Json(
                        new { Message = "Access denied. Only managers can log in or user not found." }, 
                        statusCode: 403
                    );
                }

                return Results.Ok(new LoginResponseDto(token));
            })
            .WithTags("Authentication");
    }
}