using Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Barber.Api.Endpoints;

public static class ShopEndpoints
{
    public static void MapShopEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/shops", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await db.Shops
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToPagedResultAsync(page, pageSize));
    }
}
