using Instartups.Command.Application.Common;

namespace Instartups.Command.Application.UnitTests.Common;

public class ValidarUrlTests
{
    [Theory]
    [InlineData("http://instartups.com")]
    [InlineData("https://instartups.com")]
    [InlineData("https://cdn.instartups.com/imagens/perfil.png?v=2")]
    [InlineData("HTTPS://INSTARTUPS.COM")]
    [InlineData("http://localhost:5000/imagem.png")]
    public void Valida_QuandoUrlAbsolutaHttpOuHttps_DeveRetornarVerdadeiro(string url)
    {
        ValidarUrl.Valida(url).ShouldBeTrue();
    }

    [Theory]
    [InlineData("ftp://instartups.com/arquivo.png")]
    [InlineData("file:///tmp/imagem.png")]
    [InlineData("mailto:contato@instartups.com")]
    [InlineData("/imagens/perfil.png")]
    [InlineData("instartups.com")]
    [InlineData("nao eh uma url")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Valida_QuandoUrlInvalidaRelativaOuComOutroEsquema_DeveRetornarFalso(string? url)
    {
        ValidarUrl.Valida(url).ShouldBeFalse();
    }
}
