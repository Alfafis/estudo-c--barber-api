namespace Barber.Application.DTOs;

public record CreateAppointmentRequest(
    Guid ClientId,
    Guid BarberId,
    Guid ServiceId,
    DateTimeOffset StartTime);