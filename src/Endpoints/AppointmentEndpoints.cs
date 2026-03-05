using Barber.Application.Services;
using Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Barber.Api.Endpoints;

public static class AppointmentEndpoints
{
    public static void MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/appointments", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await db.Appointments
                .AsNoTracking()
                .OrderBy(x => x.StartTime)
                .ToPagedResultAsync(page, pageSize));

        app.MapGet("/appointments/{barberId:guid}", async (Guid barberId, AppointmentService service) => 
        {
            var agenda = await service.GetDailyAgenda(barberId);
            return Results.Ok(agenda);
        });
    }
}