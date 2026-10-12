using System.Globalization;
using FluentValidation;
using FluentValidation.TestHelper;
using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Application.UnitTests.Common.Validators;

public abstract class StartupCommandValidatorTestsBase<TCommand>
    where TCommand : class
{
    protected abstract IValidator<TCommand> Validator { get; }
    protected abstract TCommand CommandValido();
    protected abstract TCommand ComNomePerfil(string nome);
    protected abstract TCommand ComLatitudePerfil(double latitude);
    protected abstract TCommand SemPerfil();
    protected abstract TCommand ComPitch(string pitch);
    protected abstract TCommand ComDataFundacao(DateOnly dataFundacao);
    protected abstract TCommand ComTamanhoEquipe(int tamanhoEquipe);
    protected abstract TCommand ComValorBuscado(decimal valorBuscado);

    private TestValidationResult<TCommand> Validar(TCommand command) => Validator.TestValidate(command);

    private static DateOnly HojeUtc => DateOnly.FromDateTime(DateTime.UtcNow);

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
        Validar(ComNomePerfil(""))
            .ShouldHaveValidationErrorFor("Perfil.Nome")
            .WithErrorCode(CodigosErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoCoordenadaDoPerfilInvalida_DeveRetornarErroNaPropriedadeAninhada()
    {
        Validar(ComLatitudePerfil(-91))
            .ShouldHaveValidationErrorFor("Perfil.Coordenada.Latitude")
            .WithErrorCode(CodigosErroConst.ValorEntre);
    }

    #region Pitch

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validar_QuandoPitchVazio_DeveRetornarErroCampoObrigatorio(string pitch)
    {
        Validar(ComPitch(pitch))
            .ShouldHaveValidationErrorFor("Pitch")
            .WithErrorCode(CodigosErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoPitchComTamanhoMaximo_NaoDeveRetornarErro()
    {
        Validar(ComPitch(new string('a', TamanhosColunasConst.Startup.Pitch)))
            .ShouldNotHaveValidationErrorFor("Pitch");
    }

    [Fact]
    public void Validar_QuandoPitchExcedeTamanhoMaximo_DeveRetornarErroTamanhoMaximo()
    {
        Validar(ComPitch(new string('a', TamanhosColunasConst.Startup.Pitch + 1)))
            .ShouldHaveValidationErrorFor("Pitch")
            .WithErrorCode(CodigosErroConst.TamanhoMaximo)
            .WithErrorMessage("O tamanho do campo deve ser no máximo 1000.");
    }

    #endregion

    #region DataFundacao

    [Fact]
    public void Validar_QuandoDataFundacaoHoje_NaoDeveRetornarErro()
    {
        Validar(ComDataFundacao(HojeUtc)).ShouldNotHaveValidationErrorFor("DataFundacao");
    }

    [Fact]
    public void Validar_QuandoDataFundacaoNoPassado_NaoDeveRetornarErro()
    {
        Validar(ComDataFundacao(new DateOnly(1990, 5, 20))).ShouldNotHaveValidationErrorFor("DataFundacao");
    }

    [Fact]
    public void Validar_QuandoDataFundacaoNoFuturo_DeveRetornarErroDataFutura()
    {
        Validar(ComDataFundacao(HojeUtc.AddDays(1)))
            .ShouldHaveValidationErrorFor("DataFundacao")
            .WithErrorCode(CodigosErroConst.DataFutura)
            .WithErrorMessage(MensagensErroConst.DataFutura);
    }

    [Fact]
    public void Validar_QuandoDataFundacaoNaoInformada_DeveRetornarErroCampoObrigatorio()
    {
        Validar(ComDataFundacao(default))
            .ShouldHaveValidationErrorFor("DataFundacao")
            .WithErrorCode(CodigosErroConst.CampoObrigatorio);
    }

    #endregion

    #region TamanhoEquipe

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(short.MaxValue)]
    public void Validar_QuandoTamanhoEquipeDentroDoIntervalo_NaoDeveRetornarErro(int tamanhoEquipe)
    {
        Validar(ComTamanhoEquipe(tamanhoEquipe)).ShouldNotHaveValidationErrorFor("TamanhoEquipe");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(short.MaxValue + 1)]
    public void Validar_QuandoTamanhoEquipeForaDoIntervalo_DeveRetornarErroValorEntre(int tamanhoEquipe)
    {
        Validar(ComTamanhoEquipe(tamanhoEquipe))
            .ShouldHaveValidationErrorFor("TamanhoEquipe")
            .WithErrorCode(CodigosErroConst.ValorEntre)
            .WithErrorMessage("O valor do campo deve estar entre 1 e 32767.");
    }

    #endregion

    #region ValorBuscado

    [Theory]
    [InlineData("0.01")]
    [InlineData("1.10")]
    [InlineData("1.100")]
    [InlineData("9999999999.99")]
    public void Validar_QuandoValorBuscadoValido_NaoDeveRetornarErro(string valor)
    {
        Validar(ComValorBuscado(Decimal(valor)))
            .ShouldNotHaveValidationErrorFor("ValorBuscado");
    }

    [Fact]
    public void Validar_QuandoValorBuscadoZero_DeveRetornarErrosCampoObrigatorioEValorMinimo()
    {
        var resultado = Validar(ComValorBuscado(0));

        resultado.ShouldHaveValidationErrorFor("ValorBuscado").WithErrorCode(CodigosErroConst.CampoObrigatorio);
        resultado.ShouldHaveValidationErrorFor("ValorBuscado").WithErrorCode(CodigosErroConst.ValorMinimo);
        resultado.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public void Validar_QuandoValorBuscadoNegativo_DeveRetornarApenasErroValorMinimo()
    {
        var resultado = Validar(ComValorBuscado(-100));

        var erro = resultado.Errors.ShouldHaveSingleItem();
        erro.PropertyName.ShouldBe("ValorBuscado");
        erro.ErrorCode.ShouldBe(CodigosErroConst.ValorMinimo);
        erro.ErrorMessage.ShouldBe("O valor do campo deve ser maior que 0.");
    }

    [Theory]
    [InlineData("10000000000")]
    [InlineData("1.001")]
    [InlineData("123.456")]
    public void Validar_QuandoValorBuscadoExcedePrecisaoOuEscala_DeveRetornarErroPrecisaoIncorreta(string valor)
    {
        Validar(ComValorBuscado(Decimal(valor)))
            .ShouldHaveValidationErrorFor("ValorBuscado")
            .WithErrorCode(CodigosErroConst.PrecisaoIncorreta)
            .WithErrorMessage(MensagensErroConst.PrecisaoIncorreta);
    }

    #endregion
}
