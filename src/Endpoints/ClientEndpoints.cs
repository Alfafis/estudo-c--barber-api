using Barber.Application.DTOs;
using Barber.Application.Services;
using Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Barber.Api.Endpoints;

public static class ClientEndpoints
{
    public static void MapClientEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/clients", async (BarberDbContext db, int page = 1, int pageSize = 10) =>
            await db.Clients
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToPagedResultAsync(page, pageSize));

        app.MapPost("/clients/register", async (CreateClientRequest request, ClientService service) =>
        {
            try {
                var result = await service.RegisterClientAsync(request);
                return Results.Ok(result);
            }
            catch (Exception ex) {
                return Results.BadRequest(new { error = ex.Message });
            }
        });
    }
}