using Barber.Application.DTOs;
using Barber.Application.Services;

namespace Barber.Api.Endpoints;

public static class BookingEndpoints
{
    public static void MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/bookings", async (CreateAppointmentRequest request, BookingService service) =>
        {
            try {
                var result = await service.BookServiceAsync(request);
                return Results.Ok(result);
            }
            catch (Exception ex) {
                return Results.Conflict(new { message = ex.Message });
            }
        });

        app.MapPatch("/bookings/{id:guid}/cancel", async (Guid id, BookingService service) =>
        {
            try {
                var result = await service.CancelBookingAsync(id);
                return Results.Ok(result);
            }
            catch (Exception ex) {
                return Results.BadRequest(new { message = ex.Message });
            }
        });
    }
}