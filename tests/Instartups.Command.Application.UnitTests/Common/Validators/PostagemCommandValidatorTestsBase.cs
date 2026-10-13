using FluentValidation;
using FluentValidation.TestHelper;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Tests.Common.Builders.Commands;

namespace Instartups.Command.Application.UnitTests.Common.Validators;

public abstract class PostagemCommandValidatorTestsBase<TCommand>
    where TCommand : class
{
    protected abstract IValidator<TCommand> Validator { get; }
    protected abstract string PropriedadeMidias { get; }
    protected abstract TCommand CommandValido();
    protected abstract TCommand ComDescricao(string descricao);
    protected abstract TCommand ComMidias(IEnumerable<MidiaCommand> midias);

    protected TestValidationResult<TCommand> Validar(TCommand command) => Validator.TestValidate(command);

    private TestValidationResult<TCommand> ValidarMidia(MidiaCommand midia) => Validar(ComMidias([midia]));

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

    #region Descricao

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validar_QuandoDescricaoVazia_DeveRetornarErroCampoObrigatorio(string descricao)
    {
        Validar(ComDescricao(descricao))
            .ShouldHaveValidationErrorFor("Descricao")
            .WithErrorCode(CodigosErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoDescricaoComTamanhoMaximo_NaoDeveRetornarErro()
    {
        Validar(ComDescricao(new string('a', TamanhosColunasConst.Postagem.Descricao)))
            .ShouldNotHaveValidationErrorFor("Descricao");
    }

    [Fact]
    public void Validar_QuandoDescricaoExcedeTamanhoMaximo_DeveRetornarErroTamanhoMaximo()
    {
        Validar(ComDescricao(new string('a', TamanhosColunasConst.Postagem.Descricao + 1)))
            .ShouldHaveValidationErrorFor("Descricao")
            .WithErrorCode(CodigosErroConst.TamanhoMaximo)
            .WithErrorMessage("O tamanho do campo deve ser no máximo 500.");
    }

    #endregion

    #region Midias

    [Fact]
    public void Validar_QuandoSemMidias_NaoDeveRetornarErros()
    {
        Validar(ComMidias([])).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_QuandoMaisDeCincoMidias_NaoDeveRetornarErros()
    {
        Validar(ComMidias(PostagemCommandBuilder.Midias(6))).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(TiposMidiaEnum.Foto)]
    [InlineData(TiposMidiaEnum.Video)]
    public void Validar_QuandoMidiaComTipoValido_NaoDeveRetornarErros(TiposMidiaEnum tipo)
    {
        ValidarMidia(PostagemCommandBuilder.Midia(tipo: tipo)).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_QuandoMidiaNula_DeveRetornarErroCampoObrigatorio()
    {
        Validar(ComMidias([null!]))
            .ShouldHaveValidationErrorFor($"{PropriedadeMidias}[0]")
            .WithErrorCode(CodigosErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoUrlDaMidiaVazia_DeveRetornarErrosCampoObrigatorioEUrlInvalida()
    {
        var resultado = ValidarMidia(new MidiaCommand("", TiposMidiaEnum.Foto));

        resultado.ShouldHaveValidationErrorFor($"{PropriedadeMidias}[0].Url").WithErrorCode(CodigosErroConst.CampoObrigatorio);
        resultado.ShouldHaveValidationErrorFor($"{PropriedadeMidias}[0].Url").WithErrorCode(CodigosErroConst.UrlInvalida);
    }

    [Theory]
    [InlineData("midia.png")]
    [InlineData("ftp://cdn.instartups.com/midia.png")]
    public void Validar_QuandoUrlDaMidiaInvalida_DeveRetornarErroUrlInvalida(string url)
    {
        ValidarMidia(new MidiaCommand(url, TiposMidiaEnum.Foto))
            .ShouldHaveValidationErrorFor($"{PropriedadeMidias}[0].Url")
            .WithErrorCode(CodigosErroConst.UrlInvalida);
    }

    [Fact]
    public void Validar_QuandoUrlDaMidiaComTamanhoMaximo_NaoDeveRetornarErros()
    {
        ValidarMidia(new MidiaCommand(UrlComTamanho(TamanhosColunasConst.MidiaPostagem.Url), TiposMidiaEnum.Foto))
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_QuandoUrlDaMidiaExcedeTamanhoMaximo_DeveRetornarErroTamanhoMaximo()
    {
        ValidarMidia(new MidiaCommand(UrlComTamanho(TamanhosColunasConst.MidiaPostagem.Url + 1), TiposMidiaEnum.Foto))
            .ShouldHaveValidationErrorFor($"{PropriedadeMidias}[0].Url")
            .WithErrorCode(CodigosErroConst.TamanhoMaximo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(-1)]
    public void Validar_QuandoTipoDaMidiaInvalido_DeveRetornarErroValorInvalidoEnum(int tipo)
    {
        ValidarMidia(new MidiaCommand("https://cdn.instartups.com/midia.png", (TiposMidiaEnum)tipo))
            .ShouldHaveValidationErrorFor($"{PropriedadeMidias}[0].Tipo")
            .WithErrorCode(CodigosErroConst.ValorInvalidoEnum)
            .WithErrorMessage(MensagensErroConst.ValorInvalidoEnum);
    }

    [Fact]
    public void Validar_QuandoSegundaMidiaInvalida_DeveRetornarErroComIndiceDaMidia()
    {
        var resultado = Validar(ComMidias([PostagemCommandBuilder.Midia(0), new MidiaCommand("invalida", TiposMidiaEnum.Video)]));

        resultado.ShouldHaveValidationErrorFor($"{PropriedadeMidias}[1].Url");
        resultado.ShouldNotHaveValidationErrorFor($"{PropriedadeMidias}[0].Url");
    }

    #endregion
}
