using Barber.Domain.Entities;
using Barber.Domain.Repositories;
using Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Barber.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BarberDbContext _context;
    public UserRepository(BarberDbContext context) => _context = context;

    public async Task AddAsync(User user) => await _context.Users.AddAsync(user);

    public async Task<bool> EmailExistsAsync(string email) 
        => await _context.Users.AnyAsync(u => u.Email == email);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}