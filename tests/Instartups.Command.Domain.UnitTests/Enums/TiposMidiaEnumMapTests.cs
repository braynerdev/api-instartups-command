using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Enums.Mapear;

namespace Instartups.Command.Domain.UnitTests.Enums;

public class TiposMidiaEnumMapTests
{
    [Theory]
    [InlineData(TiposMidiaEnum.Foto, "FOTO")]
    [InlineData(TiposMidiaEnum.Video, "VÍDEO")]
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
    [InlineData("FOTO", TiposMidiaEnum.Foto)]
    [InlineData("foto", TiposMidiaEnum.Foto)]
    [InlineData("  Foto  ", TiposMidiaEnum.Foto)]
    [InlineData("VIDEO", TiposMidiaEnum.Video)]
    [InlineData("video", TiposMidiaEnum.Video)]
    [InlineData(" Video ", TiposMidiaEnum.Video)]
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
