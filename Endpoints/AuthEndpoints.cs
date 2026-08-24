
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using RestaurantReservation.API.DTOs;
using RestaurantReservation.API.Services.Auth;

namespace RestaurantReservation.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/login", async (LoginRequestDto dto, IAuth authService) =>
            {
                var token = await authService.LoginAsync(dto.EmployeeName);

                if (token is null)
                {
                    return Results.Json(
                        new { Message = "Access denied. Only managers can log in or user not found." },
                        statusCode: StatusCodes.Status403Forbidden
                    );
                }

                return Results.Ok(new LoginResponseDto(token));
            })
            .Produces<LoginResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden);
        
        group.MapPost("/logout", async (ClaimsPrincipal user, IAuth authService) =>
        {
            var jti = user.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            var exp = user.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
            var success = await authService.LogoutAsync(jti, exp);
            return success 
                ? Results.Ok(new { Message = "Logged out successfully." }) 
                : Results.BadRequest(new { Message = "Invalid token or claims." });
        })
        .RequireAuthorization();
        
        group.MapPost("/logout-all-devices", async (ClaimsPrincipal user, IAuth authService) =>
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                             ?? user.FindFirst("sub")?.Value;

                var success = await authService.LogoutAllDevicesAsync(userId);

                return success 
                    ? Results.Ok(new { Message = "Logged out from all devices successfully." }) 
                    : Results.BadRequest(new { Message = "Invalid user claim." });
            })
            .RequireAuthorization();}
}