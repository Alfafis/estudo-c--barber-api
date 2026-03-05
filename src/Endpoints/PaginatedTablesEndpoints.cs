using Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Barber.Api.Endpoints;

public static class PaginatedTablesEndpoints
{
    public static void MapPaginatedTablesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/shops", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await ToPagedResultAsync(
                db.Shops
                    .AsNoTracking()
                    .OrderBy(x => x.Id),
                page,
                pageSize));

        app.MapGet("/users", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await ToPagedResultAsync(
                db.Users
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.Email,
                        x.Role
                    }),
                page,
                pageSize));

        app.MapGet("/barbers", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await ToPagedResultAsync(
                db.Barbers
                    .AsNoTracking()
                    .OrderBy(x => x.Id),
                page,
                pageSize));

        app.MapGet("/clients", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await ToPagedResultAsync(
                db.Clients
                    .AsNoTracking()
                    .OrderBy(x => x.Id),
                page,
                pageSize));

        app.MapGet("/services", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await ToPagedResultAsync(
                db.Services
                    .AsNoTracking()
                    .OrderBy(x => x.Id),
                page,
                pageSize));

        app.MapGet("/appointments", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await ToPagedResultAsync(
                db.Appointments
                    .AsNoTracking()
                    .OrderBy(x => x.StartTime),
                page,
                pageSize));

        app.MapGet("/orders", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await ToPagedResultAsync(
                db.Orders
                    .AsNoTracking()
                    .OrderBy(x => x.StartTime),
                page,
                pageSize));

        app.MapGet("/transactions", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await ToPagedResultAsync(
                db.Transactions
                    .AsNoTracking()
                    .OrderBy(x => x.Id),
                page,
                pageSize));
    }

    private static async Task<IResult> ToPagedResultAsync<T>(IQueryable<T> query, int page, int pageSize)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);

        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Results.Ok(new
        {
            data,
            meta = new
            {
                currentPage = page,
                pageSize,
                totalItems,
                totalPages,
                hasNext = page < totalPages,
                hasPrevious = page > 1
            }
        });
    }

    private static (int page, int pageSize) NormalizePagination(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? 10 : pageSize;

        if (normalizedPageSize > 100)
        {
            normalizedPageSize = 100;
        }

        return (normalizedPage, normalizedPageSize);
    }
}
