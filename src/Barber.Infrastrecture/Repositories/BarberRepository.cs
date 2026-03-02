using Barber.Domain.Entities;
using Barber.Domain.Repositories;
using Barber.Infrastructure.Context;

namespace Barber.Infrastructure.Repositories;

public class BarberRepository : IBarberRepository
{
    private readonly BarberDbContext _context;
    public BarberRepository(BarberDbContext context) => _context = context;

    public async Task AddAsync(global::Barber.Domain.Entities.Barber barber) => await _context.Barbers.AddAsync(barber);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    
}