using Barber.Domain.Entities;

namespace Barber.Domain.Repositories;

public interface IAppointmentRepository
{
    Task<IEnumerable<Appointment>> GetTodayAppointmentsAsync(Guid barberId);
    Task<bool> HasConflictAsync(Guid barberId, DateTimeOffset start, DateTimeOffset end);
    Task AddAsync(Order order);
    Task SaveChangesAsync();
}