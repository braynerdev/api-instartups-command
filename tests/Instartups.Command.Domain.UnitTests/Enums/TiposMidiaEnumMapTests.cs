using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Enums.Mapear;

namespace Instartups.Command.Domain.UnitTests.Enums;

public class TiposMidiaEnumMapTests
{
    [Theory]
    [InlineData(TiposMidiaEnum.FOTO, "FOTO")]
    [InlineData(TiposMidiaEnum.VIDEO, "VÍDEO")]
    public void Map_QuandoTipoValido_DeveRetornarDescricao(TiposMidiaEnum tipo, string esperado)
    {
        tipo.Map().ShouldBe(esperado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(99)]
    public void Map_QuandoTipoInvalido_DeveRetornarErro(int valor)
    {
        ((TiposMidiaEnum)valor).Map().ShouldBe("ERRO");
    }

    [Theory]
    [InlineData("FOTO", TiposMidiaEnum.FOTO)]
    [InlineData("foto", TiposMidiaEnum.FOTO)]
    [InlineData("  Foto  ", TiposMidiaEnum.FOTO)]
    [InlineData("VIDEO", TiposMidiaEnum.VIDEO)]
    [InlineData("video", TiposMidiaEnum.VIDEO)]
    [InlineData(" Video ", TiposMidiaEnum.VIDEO)]
    public void MapTiposMidia_QuandoTextoValido_DeveIgnorarCaixaEEspacos(string texto, TiposMidiaEnum esperado)
    {
        texto.MapTiposMidia().ShouldBe(esperado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("AUDIO")]
    [InlineData("VÍDEO")]
    public void MapTiposMidia_QuandoTextoDesconhecido_DeveRetornarValorZero(string texto)
    {
        var resultado = texto.MapTiposMidia();

        ((int)resultado).ShouldBe(0);
        Enum.IsDefined(resultado).ShouldBeFalse();
    }

    [Fact]
    public void MapTiposMidia_QuandoTextoNulo_DeveLancarNullReferenceException()
    {
        string texto = null!;

        Action acao = () => texto.MapTiposMidia();

        acao.ShouldThrow<NullReferenceException>();
    }
}
