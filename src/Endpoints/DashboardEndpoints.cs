using Barber.Application.Services;

namespace Barber.Api.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/daily", async (DashboardService service) =>
        {
            var stats = await service.GetDailyStatsAsync();
            return Results.Ok(stats);
        });
    }
}