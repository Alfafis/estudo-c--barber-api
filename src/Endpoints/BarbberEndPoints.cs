using Barber.Application.DTOs;
using Barber.Application.Services;

namespace Barber.Api.Endpoints;

public static class BarberEndpoints
{
    public static void MapBarberEndpoints(this IEndpointRouteBuilder app)
    {
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