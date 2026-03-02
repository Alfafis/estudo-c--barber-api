using Barber.Application.DTOs;
using Barber.Application.Services;

namespace Barber.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/login", async (LoginRequest request, AuthService authService) =>
        {
            var result = await authService.AuthenticateAsync(request);
            return result is null ? Results.Unauthorized() : Results.Ok(result);
        });

        group.MapGet("/me", () => "Dados do usuário logado");
    }
}