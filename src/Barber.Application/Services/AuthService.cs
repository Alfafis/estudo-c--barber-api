using Barber.Infrastructure.Context;
using Barber.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Barber.Application.Services;

public class AuthService
{
    private readonly BarberDbContext _context;

    public AuthService(BarberDbContext context)
    {
        _context = context;
    }

    public async Task<object?> AuthenticateAsync(LoginRequest request)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || user.Password != request.Password)
            return null;

        return new 
        {
            user.Id,
            user.Name,
            user.Email,
            user.Role,
            LoginAt = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-3)).ToString("dd/MM/yyyy HH:mm")
        };
    }
}