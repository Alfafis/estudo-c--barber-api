using Npgsql;

namespace Barber.Api.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
      app.MapGet("/", () => Results.Ok(new { service = "barber-api", status = "running" }));
      app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-3)).ToString("dd/MM/yyyy HH:mm") }));
      app.MapGet("/db-health", async (NpgsqlDataSource dataSource) =>
      {
        try
        {
          await using var connection = await dataSource.OpenConnectionAsync();
          await using var command = new NpgsqlCommand("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public';", connection);
          var tablesCount = (long)(await command.ExecuteScalarAsync() ?? 0L);
          return Results.Ok(new
          {
            status = "database-connected",
            database = connection.Database,
            host = connection.Host,
            tablesInPublicSchema = tablesCount,
            timestamp =DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-3)).ToString("dd/MM/yyyy HH:mm")
          });
        }
        catch (Exception ex)
        {
          return Results.Problem(
            detail: ex.Message,
            title: "Database connection failed",
            statusCode: StatusCodes.Status503ServiceUnavailable);
        }
      });
    }
}