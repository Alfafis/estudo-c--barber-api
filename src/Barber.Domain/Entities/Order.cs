namespace Barber.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid BarberId { get; private set; }
    public Guid ServiceId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    public string Status { get; private set; } = "pending";
    public string? ReferenceDocument { get; private set; }

    private Order() { }

    public Order(Guid clientId, Guid barberId, Guid serviceId, decimal totalAmount, DateTimeOffset start, DateTimeOffset end)
    {
        Id = Guid.NewGuid();
        ClientId = clientId;
        BarberId = barberId;
        ServiceId = serviceId;
        TotalAmount = totalAmount;
        StartTime = start;
        EndTime = end;
    }

    public void UpdateStatus(string status)
    {
        Status = status;
    }
}