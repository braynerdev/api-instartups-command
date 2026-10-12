using Instartups.Command.Domain.ValueObjects;

namespace Instartups.Command.Domain.UnitTests.ValueObjects;

public class NomeVOTests
{
    [Fact]
    public void Create_DeveGuardarONomeInformado()
    {
        var nome = NomeVO.Create("Instartups");

        nome.Nome.ShouldBe("Instartups");
    }

    [Theory]
    [InlineData("  Instartups", "Instartups")]
    [InlineData("Instartups  ", "Instartups")]
    [InlineData("\t Instartups \n", "Instartups")]
    public void Create_QuandoNomeTemEspacosNasPontas_DeveRemoverOsEspacos(string entrada, string esperado)
    {
        var nome = NomeVO.Create(entrada);

        nome.Nome.ShouldBe(esperado);
    }

    [Fact]
    public void Create_QuandoNomeTemEspacosInternos_DevePreservarOsEspacos()
    {
        var nome = NomeVO.Create("  Instartups   Tech  ");

        nome.Nome.ShouldBe("Instartups   Tech");
    }

    [Fact]
    public void Equals_QuandoNomesIguaisAposTrim_DeveSerIgual()
    {
        NomeVO.Create(" Instartups ").ShouldBe(NomeVO.Create("Instartups"));
    }

    [Fact]
    public void Equals_QuandoNomesDiferentes_NaoDeveSerIgual()
    {
        NomeVO.Create("Instartups").ShouldNotBe(NomeVO.Create("instartups"));
    }
}
