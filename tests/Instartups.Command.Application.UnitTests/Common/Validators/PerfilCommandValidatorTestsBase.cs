using FluentValidation;
using FluentValidation.TestHelper;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UnitTests.Common.Validators;

public abstract class PerfilCommandValidatorTestsBase<TCommand>
    where TCommand : class
{
    protected abstract IValidator<TCommand> Validator { get; }
    protected abstract TCommand CommandValido();
    protected abstract TCommand ComNome(string nome);
    protected abstract TCommand ComImagemPerfilUrl(string? url);
    protected abstract TCommand ComImagemFundoUrl(string? url);
    protected abstract TCommand ComCoordenada(CoordenadaCommand coordenada);

    private TestValidationResult<TCommand> Validar(TCommand command) => Validator.TestValidate(command);

    private static string UrlComTamanho(int tamanho)
    {
        const string prefixo = "https://cdn.instartups.com/";
        return prefixo + new string('a', tamanho - prefixo.Length);
    }

    [Fact]
    public void Validar_QuandoCommandValido_NaoDeveRetornarErros()
    {
        Validar(CommandValido()).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validar_QuandoNomeVazio_DeveRetornarErroCampoObrigatorio(string nome)
    {
        Validar(ComNome(nome))
            .ShouldHaveValidationErrorFor("Nome")
            .WithErrorCode(CodigosErroConst.CampoObrigatorio)
            .WithErrorMessage(MensagensErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoNomeComTamanhoMaximo_NaoDeveRetornarErro()
    {
        Validar(ComNome(new string('a', TamanhosColunasConst.Perfil.Nome)))
            .ShouldNotHaveValidationErrorFor("Nome");
    }

    [Fact]
    public void Validar_QuandoNomeExcedeTamanhoMaximo_DeveRetornarErroTamanhoMaximo()
    {
        Validar(ComNome(new string('a', TamanhosColunasConst.Perfil.Nome + 1)))
            .ShouldHaveValidationErrorFor("Nome")
            .WithErrorCode(CodigosErroConst.TamanhoMaximo)
            .WithErrorMessage("O tamanho do campo deve ser no máximo 100.");
    }

    [Fact]
    public void Validar_QuandoNomeComEspacosExcedeTamanhoAntesDoTrim_DeveRetornarErroTamanhoMaximo()
    {
        Validar(ComNome($" {new string('a', TamanhosColunasConst.Perfil.Nome)} "))
            .ShouldHaveValidationErrorFor("Nome")
            .WithErrorCode(CodigosErroConst.TamanhoMaximo);
    }

    [Fact]
    public void Validar_QuandoImagensNulas_NaoDeveRetornarErro()
    {
        Validar(ComImagemPerfilUrl(null)).ShouldNotHaveValidationErrorFor("ImagemPerfilUrl");
        Validar(ComImagemFundoUrl(null)).ShouldNotHaveValidationErrorFor("ImagemFundoUrl");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("imagem.png")]
    [InlineData("/imagens/perfil.png")]
    [InlineData("ftp://cdn.instartups.com/perfil.png")]
    public void Validar_QuandoImagemPerfilUrlInvalida_DeveRetornarErroUrlInvalida(string url)
    {
        Validar(ComImagemPerfilUrl(url))
            .ShouldHaveValidationErrorFor("ImagemPerfilUrl")
            .WithErrorCode(CodigosErroConst.UrlInvalida)
            .WithErrorMessage(MensagensErroConst.UrlInvalida);
    }

    [Theory]
    [InlineData("")]
    [InlineData("imagem.png")]
    [InlineData("ftp://cdn.instartups.com/fundo.png")]
    public void Validar_QuandoImagemFundoUrlInvalida_DeveRetornarErroUrlInvalida(string url)
    {
        Validar(ComImagemFundoUrl(url))
            .ShouldHaveValidationErrorFor("ImagemFundoUrl")
            .WithErrorCode(CodigosErroConst.UrlInvalida);
    }

    [Fact]
    public void Validar_QuandoImagensComTamanhoMaximo_NaoDeveRetornarErro()
    {
        Validar(ComImagemPerfilUrl(UrlComTamanho(TamanhosColunasConst.Perfil.ImagemPerfilUrl)))
            .ShouldNotHaveValidationErrorFor("ImagemPerfilUrl");
        Validar(ComImagemFundoUrl(UrlComTamanho(TamanhosColunasConst.Perfil.ImagemFundoUrl)))
            .ShouldNotHaveValidationErrorFor("ImagemFundoUrl");
    }

    [Fact]
    public void Validar_QuandoImagemPerfilUrlExcedeTamanhoMaximo_DeveRetornarErroTamanhoMaximo()
    {
        Validar(ComImagemPerfilUrl(UrlComTamanho(TamanhosColunasConst.Perfil.ImagemPerfilUrl + 1)))
            .ShouldHaveValidationErrorFor("ImagemPerfilUrl")
            .WithErrorCode(CodigosErroConst.TamanhoMaximo)
            .WithErrorMessage("O tamanho do campo deve ser no máximo 400.");
    }

    [Fact]
    public void Validar_QuandoImagemFundoUrlExcedeTamanhoMaximo_DeveRetornarErroTamanhoMaximo()
    {
        Validar(ComImagemFundoUrl(UrlComTamanho(TamanhosColunasConst.Perfil.ImagemFundoUrl + 1)))
            .ShouldHaveValidationErrorFor("ImagemFundoUrl")
            .WithErrorCode(CodigosErroConst.TamanhoMaximo);
    }

    [Fact]
    public void Validar_QuandoCoordenadaNula_DeveRetornarErroCampoObrigatorio()
    {
        Validar(ComCoordenada(null!))
            .ShouldHaveValidationErrorFor("Coordenada")
            .WithErrorCode(CodigosErroConst.CampoObrigatorio)
            .WithErrorMessage(MensagensErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoLatitudeInvalida_DeveRetornarErroNaPropriedadeAninhada()
    {
        Validar(ComCoordenada(new CoordenadaCommand(91, 10)))
            .ShouldHaveValidationErrorFor("Coordenada.Latitude")
            .WithErrorCode(CodigosErroConst.ValorEntre);
    }

    [Fact]
    public void Validar_QuandoLongitudeZero_DeveRetornarErroNaPropriedadeAninhada()
    {
        Validar(ComCoordenada(new CoordenadaCommand(10, 0)))
            .ShouldHaveValidationErrorFor("Coordenada.Longitude")
            .WithErrorCode(CodigosErroConst.CampoObrigatorio);
    }
}
