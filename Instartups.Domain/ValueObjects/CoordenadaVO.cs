namespace Instartups.Domain.ValueObjects;

public sealed record CoordenadaVO
{
    public double Latitude { get; }
    public double Longitude { get; }

    private CoordenadaVO(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static CoordenadaVO Create(double latitude, double longitude)
    {
        return new CoordenadaVO(latitude, longitude);
    }
}
