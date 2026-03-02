using Barber.Domain.Repositories;

namespace Barber.Application.Services;

public class AppointmentService
{
    private readonly IAppointmentRepository _repo;

    public AppointmentService(IAppointmentRepository repo) => _repo = repo;

    public async Task<object> GetDailyAgenda(Guid barberId)
    {
        var appointments = await _repo.GetTodayAppointmentsAsync(barberId);

        return appointments.Select(a => new {
            a.Id,
            a.Status,
            Inicio = a.StartTime.ToOffset(TimeSpan.FromHours(-3)).ToString("HH:mm"),
            Fim = a.EndTime.ToOffset(TimeSpan.FromHours(-3)).ToString("HH:mm")
        });
    }
}