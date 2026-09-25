using System;
using System.Collections.Generic;
using System.Text;

namespace Instartups.Domain.Entities.ValueObjects;

public sealed record CoordenadasVO
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

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
