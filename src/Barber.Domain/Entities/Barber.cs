namespace Barber.Domain.Entities;

public class Barber
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string? Specialty { get; private set; }
    public string DocumentId { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    private Barber() { }

    public Barber(Guid userId, string documentId, string? specialty = null, bool isActive = true)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        DocumentId = documentId;
        Specialty = specialty;
        IsActive = isActive;
    }
}
