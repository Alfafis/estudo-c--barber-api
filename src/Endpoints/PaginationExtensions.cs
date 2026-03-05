using Microsoft.EntityFrameworkCore;

namespace Barber.Api.Endpoints;

public static class PaginationExtensions
{
    public static async Task<IResult> ToPagedResultAsync<T>(this IQueryable<T> query, int page, int pageSize)
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
