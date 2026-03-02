using Barber.Application.DTOs;
using Barber.Application.Services;

namespace Barber.Api.Endpoints;

public static class ClientEndpoints
{
    public static void MapClientEndpoints(this IEndpointRouteBuilder app)
    {
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