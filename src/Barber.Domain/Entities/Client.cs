namespace Barber.Domain.Entities;

public class Client
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Phone { get; private set; } = default!;
    public DateTime? BirthDate { get; private set; }

    private Client() { }

    public Client(Guid userId, string phone, DateTime? birthDate = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Phone = phone;
        BirthDate = birthDate;
    }
}