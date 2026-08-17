namespace RestaurantReservation.API.Extensions;

public static class ValidationFilters
{
    public static RouteHandlerBuilder ValidateId(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var id = context.Arguments.OfType<int>().FirstOrDefault();

            if (id <= 0)
            {
                return Results.BadRequest(new { Message = "ID must be greater than 0." });
            }

            return await next(context);
        });
    }
}