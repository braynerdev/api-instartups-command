using System.Globalization;
using FluentValidation;
using FluentValidation.TestHelper;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UnitTests.Common.Validators;

public abstract class InvestidorCommandValidatorTestsBase<TCommand>
    where TCommand : class
{
    protected abstract IValidator<TCommand> Validator { get; }
    protected abstract TCommand CommandValido();
    protected abstract TCommand ComNomePerfil(string nome);
    protected abstract TCommand ComLatitudePerfil(double latitude);
    protected abstract TCommand SemPerfil();
    protected abstract TCommand ComTeseInvestimento(string tese);
    protected abstract TCommand ComTickets(decimal ticketMinimo, decimal ticketMaximo);

    private TestValidationResult<TCommand> Validar(TCommand command) => Validator.TestValidate(command);

    private static decimal Decimal(string valor) => decimal.Parse(valor, CultureInfo.InvariantCulture);

    [Fact]
    public void Validar_QuandoCommandValido_NaoDeveRetornarErros()
    {
        Validar(CommandValido()).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_QuandoPerfilNulo_DeveRetornarErroCampoObrigatorio()
    {
        Validar(SemPerfil())
            .ShouldHaveValidationErrorFor("Perfil")
            .WithErrorCode(CodigosErroConst.CampoObrigatorio)
            .WithErrorMessage(MensagensErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoNomeDoPerfilVazio_DeveRetornarErroNaPropriedadeAninhada()
    {
        Validar(ComNomePerfil("   "))
            .ShouldHaveValidationErrorFor("Perfil.Nome")
            .WithErrorCode(CodigosErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoCoordenadaDoPerfilInvalida_DeveRetornarErroNaPropriedadeAninhada()
    {
        Validar(ComLatitudePerfil(91))
            .ShouldHaveValidationErrorFor("Perfil.Coordenada.Latitude")
            .WithErrorCode(CodigosErroConst.ValorEntre);
    }

    #region TeseInvestimento

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validar_QuandoTeseInvestimentoVazia_DeveRetornarErroCampoObrigatorio(string tese)
    {
        Validar(ComTeseInvestimento(tese))
            .ShouldHaveValidationErrorFor("TeseInvestimento")
            .WithErrorCode(CodigosErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoTeseInvestimentoComTamanhoMaximo_NaoDeveRetornarErro()
    {
        Validar(ComTeseInvestimento(new string('a', TamanhosColunasConst.Investidor.Tese)))
            .ShouldNotHaveValidationErrorFor("TeseInvestimento");
    }

    [Fact]
    public void Validar_QuandoTeseInvestimentoExcedeTamanhoMaximo_DeveRetornarErroTamanhoMaximo()
    {
        Validar(ComTeseInvestimento(new string('a', TamanhosColunasConst.Investidor.Tese + 1)))
            .ShouldHaveValidationErrorFor("TeseInvestimento")
            .WithErrorCode(CodigosErroConst.TamanhoMaximo)
            .WithErrorMessage("O tamanho do campo deve ser no máximo 1000.");
    }

    #endregion

    #region TicketMinimo

    [Fact]
    public void Validar_QuandoTicketMinimoZero_DeveRetornarErrosCampoObrigatorioEValorMinimo()
    {
        var resultado = Validar(ComTickets(0, 1_000m));

        resultado.ShouldHaveValidationErrorFor("TicketMinimo").WithErrorCode(CodigosErroConst.CampoObrigatorio);
        resultado.ShouldHaveValidationErrorFor("TicketMinimo").WithErrorCode(CodigosErroConst.ValorMinimo);
    }

    [Fact]
    public void Validar_QuandoTicketMinimoNegativo_DeveRetornarErroValorMinimo()
    {
        var resultado = Validar(ComTickets(-1, 1_000m));

        var erro = resultado.Errors.ShouldHaveSingleItem();
        erro.PropertyName.ShouldBe("TicketMinimo");
        erro.ErrorCode.ShouldBe(CodigosErroConst.ValorMinimo);
        erro.ErrorMessage.ShouldBe("O valor do campo deve ser maior que 0.");
    }

    [Theory]
    [InlineData("10000000000")]
    [InlineData("100.001")]
    public void Validar_QuandoTicketMinimoExcedePrecisaoOuEscala_DeveRetornarErroPrecisaoIncorreta(string valor)
    {
        Validar(ComTickets(Decimal(valor), 99_999_999_999m))
            .ShouldHaveValidationErrorFor("TicketMinimo")
            .WithErrorCode(CodigosErroConst.PrecisaoIncorreta);
    }

    #endregion

    #region TicketMaximo

    [Theory]
    [InlineData("0.01", "0.02")]
    [InlineData("1000", "1000.01")]
    [InlineData("1", "9999999999.99")]
    public void Validar_QuandoTicketMaximoMaiorQueMinimo_NaoDeveRetornarErros(string minimo, string maximo)
    {
        Validar(ComTickets(Decimal(minimo), Decimal(maximo))).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_QuandoTicketMaximoIgualAoMinimo_DeveRetornarErroValorMinimo()
    {
        Validar(ComTickets(1_000m, 1_000m))
            .ShouldHaveValidationErrorFor("TicketMaximo")
            .WithErrorCode(CodigosErroConst.ValorMinimo)
            .WithErrorMessage("O valor do campo deve ser maior que 1000.");
    }

    [Fact]
    public void Validar_QuandoTicketMaximoMenorQueMinimo_DeveRetornarErroValorMinimo()
    {
        var resultado = Validar(ComTickets(1_000m, 999m));

        resultado.ShouldHaveValidationErrorFor("TicketMaximo").WithErrorCode(CodigosErroConst.ValorMinimo);
        resultado.ShouldNotHaveValidationErrorFor("TicketMinimo");
    }

    [Fact]
    public void Validar_QuandoTicketMaximoZero_DeveRetornarErrosCampoObrigatorioEValorMinimo()
    {
        var resultado = Validar(ComTickets(1_000m, 0));

        resultado.ShouldHaveValidationErrorFor("TicketMaximo").WithErrorCode(CodigosErroConst.CampoObrigatorio);
        resultado.ShouldHaveValidationErrorFor("TicketMaximo").WithErrorCode(CodigosErroConst.ValorMinimo);
    }

    [Theory]
    [InlineData("10000000000")]
    [InlineData("2000.123")]
    public void Validar_QuandoTicketMaximoExcedePrecisaoOuEscala_DeveRetornarErroPrecisaoIncorreta(string valor)
    {
        Validar(ComTickets(1_000m, Decimal(valor)))
            .ShouldHaveValidationErrorFor("TicketMaximo")
            .WithErrorCode(CodigosErroConst.PrecisaoIncorreta);
    }

    #endregion
}
