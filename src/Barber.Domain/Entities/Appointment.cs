namespace Barber.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }
    public Guid BarberId { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    public string Status { get; private set; } = "available";

    private Appointment() { }

    public Appointment(Guid barberId, DateTimeOffset start, DateTimeOffset end)
    {
        Id = Guid.NewGuid();
        BarberId = barberId;
        StartTime = start;
        EndTime = end;
    }
}