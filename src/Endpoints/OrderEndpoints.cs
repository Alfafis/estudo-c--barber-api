using Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Barber.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/orders", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await db.Orders
                .AsNoTracking()
                .OrderBy(x => x.StartTime)
                .ToPagedResultAsync(page, pageSize));
    }
}
