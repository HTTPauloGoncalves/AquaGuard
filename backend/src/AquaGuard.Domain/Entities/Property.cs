using System;
using AquaGuard.Domain.ValueObjects;

namespace AquaGuard.Domain.Entities;

public class Property
{
    private readonly List<Station> _stations = new();
    private readonly List<Reservoir> _reservoirs = new();

    public Guid Id { get; private set; }
    public Guid OwnerId { get; set; }
    public string Name { get; private set;}
    public string? Description { get; private set; }
    public string Owner { get; private set; }
    public GeoCoordinate Coordinate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public  IReadOnlyCollection<Station> Stations => _stations;
    public IReadOnlyCollection<Reservoir> Reservoirs => _reservoirs;

    public Property(Guid id, string name)
    {
        Id = id;
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required.", nameof(name)) : name;
        Owner = "AquaGuard demo";
        Coordinate = new GeoCoordinate("0", "0");
        CreatedAt = DateTime.UtcNow;
    }

    #pragma warning disable CS8618
    protected Property() { }
    #pragma warning restore CS8618
    
    public Property(string name, string? description, string owner, string latitude, string longitude)
    {
        Id = Guid.NewGuid();
        OwnerId = Guid.NewGuid();
        Name = name;
        Description = description;
        Owner = owner;
        Coordinate = new GeoCoordinate(latitude, longitude);
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string name) => Name = name;
    public void UpdateDescription(string description) => Description = description;
    public void UpdateCoordinate(string latitude, string longitude) => Coordinate = new GeoCoordinate(latitude, longitude);
    public void UpdateOwner(string owner) => Owner = owner;
    public void addStation(Station station) => _stations.Add(station);
    public void removeStation(Station station) => _stations.Remove(station);
}
