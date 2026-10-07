using System;
using AquaGuard.Domain.ValueObjects;

namespace AquaGuard.Domain.Entities;

public class Property
{
    private readonly List<Station> _stations = new();

    public Guid OwnerId { get; set; }
    public string Name { get; private set;}
    public string? Description { get; private set; }
    public string Owner { get; private set; }
    public GeoCoordinate Coordinate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public  IReadOnlyCollection<Station> Stations => _stations;

    #pragma warning disable CS8618
    protected Property() { }
    #pragma warning restore CS8618
    
    public Property(string name, string? description, string owner, string latitude, string longitude)
    {
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