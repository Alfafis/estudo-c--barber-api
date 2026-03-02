using Barber.Infrastructure.Context;
using Barber.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Barber.Application.Services;

public class DashboardService
{
    private readonly BarberDbContext _context;

    public DashboardService(BarberDbContext context) => _context = context;

    public async Task<DashboardDto> GetDailyStatsAsync()
    {
        var today = DateTimeOffset.UtcNow.Date;

        var stats = await _context.Orders
            .AsNoTracking()
            .Where(o => o.StartTime.Date == today && o.Status != "cancelled")
            .GroupBy(o => 1)
            .Select(g => new
            {
                Count = g.Count(),
                Sum = g.Sum(o => o.TotalAmount)
            })
            .FirstOrDefaultAsync();

        var count = stats?.Count ?? 0;
        var sum = stats?.Sum ?? 0;
        var average = count > 0 ? sum / count : 0;

        return new DashboardDto(
            count,
            sum,
            average,
            today.ToString("dd/MM/yyyy")
        );
    }
}