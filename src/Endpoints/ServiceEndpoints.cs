using Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Barber.Api.Endpoints;

public static class ServiceEndpoints
{
    public static void MapServiceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/services", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await db.Services
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToPagedResultAsync(page, pageSize));
    }
}
