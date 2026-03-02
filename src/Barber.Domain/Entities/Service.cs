namespace Barber.Domain.Entities;

public class Service
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int DurationMinutes { get; private set; }
    public string? ImageUrl { get; private set; }

    private Service() { }

    public Service(string name, decimal price, int durationMinutes, string? imageUrl = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        DurationMinutes = durationMinutes;
        ImageUrl = imageUrl;
    }
}