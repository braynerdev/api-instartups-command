using Instartups.Command.Domain.ValueObjects;

namespace Instartups.Command.Domain.UnitTests.ValueObjects;

public class CoordenadaVOTests
{
    [Fact]
    public void Create_DeveGuardarLatitudeELongitude()
    {
        var coordenada = CoordenadaVO.Create(-23.5505, -46.6333);

        coordenada.Latitude.ShouldBe(-23.5505);
        coordenada.Longitude.ShouldBe(-46.6333);
    }

    [Fact]
    public void Equals_QuandoMesmaLatitudeELongitude_DeveSerIgual()
    {
        CoordenadaVO.Create(10, 20).ShouldBe(CoordenadaVO.Create(10, 20));
    }

    [Theory]
    [InlineData(10, 21)]
    [InlineData(11, 20)]
    [InlineData(20, 10)]
    public void Equals_QuandoCoordenadasDiferentes_NaoDeveSerIgual(double latitude, double longitude)
    {
        CoordenadaVO.Create(10, 20).ShouldNotBe(CoordenadaVO.Create(latitude, longitude));
    }
}
