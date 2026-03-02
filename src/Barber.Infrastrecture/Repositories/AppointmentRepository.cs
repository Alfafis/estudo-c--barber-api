using Barber.Domain.Entities;
using Barber.Domain.Repositories;
using Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Barber.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly BarberDbContext _context;

    public AppointmentRepository(BarberDbContext context) => _context = context;

    public async Task<IEnumerable<Appointment>> GetTodayAppointmentsAsync(Guid barberId)
    {
        var today = DateTimeOffset.UtcNow.Date;

        return await _context.Appointments
            .AsNoTracking()
            .Where(a => a.BarberId == barberId && a.StartTime.Date == today)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<bool> HasConflictAsync(Guid barberId, DateTimeOffset start, DateTimeOffset end)
    {
        return await _context.Orders
            .AnyAsync(o => o.BarberId == barberId &&
            o.Status != "cancelled" &&
            start < o.EndTime && end > o.StartTime);
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}