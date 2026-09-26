namespace Instartups.Domain.ValueObjects;

public sealed record CoordenadasVO
{
    public double Latitude { get; }
    public double Longitude { get; }

    private CoordenadasVO() { }
    private CoordenadasVO(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static CoordenadasVO Create(double latitude, double longitude)
    {
        return new CoordenadasVO(latitude, longitude);
    }
}
