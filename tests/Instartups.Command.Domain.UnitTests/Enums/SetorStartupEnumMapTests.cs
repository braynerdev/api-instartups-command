using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Enums.Mapear;

namespace Instartups.Command.Domain.UnitTests.Enums;

public class SetorStartupEnumMapTests
{
    [Theory]
    [InlineData(SetorStartupEnum.Financas, "FINANCAS")]
    [InlineData(SetorStartupEnum.Saude, "SAUDE")]
    [InlineData(SetorStartupEnum.Agronegocios, "AGRONEGOCIOS")]
    [InlineData(SetorStartupEnum.Varejo, "VAREJO")]
    public void Map_QuandoSetorValido_DeveRetornarDescricao(SetorStartupEnum setor, string esperado)
    {
        setor.Map().ShouldBe(esperado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(99)]
    public void Map_QuandoSetorInvalido_DeveRetornarErro(int valor)
    {
        ((SetorStartupEnum)valor).Map().ShouldBe("ERRO");
    }

    [Theory]
    [InlineData("FINANCAS", SetorStartupEnum.Financas)]
    [InlineData(" finanças ", SetorStartupEnum.Financas)]
    [InlineData("SAUDE", SetorStartupEnum.Saude)]
    [InlineData("Saúde", SetorStartupEnum.Saude)]
    [InlineData("agronegocios", SetorStartupEnum.Agronegocios)]
    [InlineData("AGRONEGÓCIOS", SetorStartupEnum.Agronegocios)]
    [InlineData("  Varejo  ", SetorStartupEnum.Varejo)]
    public void MapSetorStartup_QuandoTextoValido_DeveIgnorarCaixaEspacosEAcentos(string texto, SetorStartupEnum esperado)
    {
        texto.MapSetorStartup().ShouldBe(esperado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("TECNOLOGIA")]
    public void MapSetorStartup_QuandoTextoDesconhecido_DeveRetornarValorZero(string texto)
    {
        var resultado = texto.MapSetorStartup();

        ((int)resultado).ShouldBe(0);
        Enum.IsDefined(resultado).ShouldBeFalse();
    }

    [Fact]
    public void MapSetorStartup_QuandoTextoNulo_DeveLancarNullReferenceException()
    {
        string texto = null!;

        Action acao = () => texto.MapSetorStartup();

        acao.ShouldThrow<NullReferenceException>();
    }
}
