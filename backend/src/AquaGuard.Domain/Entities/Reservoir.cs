namespace AquaGuard.Domain.Entities;

public sealed class Reservoir
{
    private Reservoir() { }

    public Reservoir(Guid id, Guid propertyId, string name, string levelReferenceLabel)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(levelReferenceLabel)) throw new ArgumentException("A level reference label is required.", nameof(levelReferenceLabel));
        Id = id;
        PropertyId = propertyId;
        Name = name;
        LevelReferenceLabel = levelReferenceLabel;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public Property? Property { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string LevelReferenceLabel { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
}
