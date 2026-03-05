using Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Barber.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await db.Users
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Email,
                    x.Role
                })
                .ToPagedResultAsync(page, pageSize));
    }
}
