using RestaurantReservation.API.Extensions;
using RestaurantReservation.API.Services.Employees;

namespace RestaurantReservation.API.Endpoints;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees")
            .WithTags("Employees")
            .RequireAuthorization(policy => policy.RequireRole("Manager"));

        group.MapGet("/managers", async (IEmployeeService employeeService) =>
            {
                var managers = await employeeService.GetManagersAsync();
                return Results.Ok(managers);
            })
            .WithName("GetManagers")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapGet("/{employeeId:int}/average-order-amount", async (int employeeId, IEmployeeService employeeService) =>
            {
                var averageAmount = await employeeService.GetAverageOrderAmountAsync(employeeId);
                return Results.Ok(new { EmployeeId = employeeId, AverageOrderAmount = averageAmount });
            })
            .ValidateId()
            .WithName("GetEmployeeAverageOrderAmount")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}