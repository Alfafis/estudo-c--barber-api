using Barber.Application.DTOs;
using Barber.Application.Services;
using Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Barber.Api.Endpoints;

public static class BarberEndpoints
{
    public static void MapBarberEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/barbers", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await db.Barbers
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToPagedResultAsync(page, pageSize));

        app.MapPost("/barbers/register", async (CreateBarberRequest request, BarberService service) =>
        {
            try {
                var result = await service.RegisterBarberAsync(request);
                return Results.Ok(result);
            }
            catch (Exception ex) {
                return Results.BadRequest(new { error = ex.Message });
            }
        });
    }
}