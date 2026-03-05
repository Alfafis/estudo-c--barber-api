using Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Barber.Api.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/transactions", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await db.Transactions
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToPagedResultAsync(page, pageSize));
    }
}
