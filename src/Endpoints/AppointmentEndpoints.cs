using Barber.Application.Services;

namespace Barber.Api.Endpoints;

public static class AppointmentEndpoints
{
    public static void MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/appointments/{barberId:guid}", async (Guid barberId, AppointmentService service) => 
        {
            var agenda = await service.GetDailyAgenda(barberId);
            return Results.Ok(agenda);
        });
    }
}