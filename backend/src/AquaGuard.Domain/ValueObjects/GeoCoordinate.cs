using System;

namespace AquaGuard.Domain.ValueObjects;

public class GeoCoordinate
{
    public string Latitude { get; private set; }
    public string Longitude { get; private set; }

    public GeoCoordinate(string latitude, string longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}
