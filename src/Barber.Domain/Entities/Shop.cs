namespace Barber.Domain.Entities;

public class Shop
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string OpeningTime { get; private set; } = default!;
    public string ClosingTime { get; private set; } = default!;
    public string? Address { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Shop() { }

    public Shop(string name, string openingTime, string closingTime, string? address)
    {
        Id = Guid.NewGuid();
        Name = name;
        OpeningTime = openingTime;
        ClosingTime = closingTime;
        Address = address;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}