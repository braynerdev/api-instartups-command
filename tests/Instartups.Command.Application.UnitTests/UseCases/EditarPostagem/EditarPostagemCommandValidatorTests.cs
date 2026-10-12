using FluentValidation;
using FluentValidation.TestHelper;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.UnitTests.Common.Validators;
using Instartups.Command.Application.UseCases.EditarPostagem;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Tests.Common.Builders.Commands;

namespace Instartups.Command.Application.UnitTests.UseCases.EditarPostagem;

public class EditarPostagemCommandValidatorTests : PostagemCommandValidatorTestsBase<EditarPostagemCommand>
{
    protected override IValidator<EditarPostagemCommand> Validator { get; } = new EditarPostagemCommandValidator();
    protected override string PropriedadeMidias => nameof(EditarPostagemCommand.MidiasAdicionadas);

    protected override EditarPostagemCommand CommandValido() => PostagemCommandBuilder.EditarPostagem() with
    {
        MidiasRemovidas = [Guid.NewGuid()],
        MidiasAdicionadas = PostagemCommandBuilder.Midias(1)
    };

    protected override EditarPostagemCommand ComDescricao(string descricao) => CommandValido() with { Descricao = descricao };
    protected override EditarPostagemCommand ComMidias(IEnumerable<MidiaCommand> midias) => CommandValido() with { MidiasAdicionadas = midias };

    [Fact]
    public void Validar_QuandoPostagemIdVazio_DeveRetornarErroCampoObrigatorio()
    {
        Validar(CommandValido() with { PostagemId = Guid.Empty })
            .ShouldHaveValidationErrorFor(x => x.PostagemId)
            .WithErrorCode(CodigosErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoUserIdVazio_DeveRetornarErroCampoObrigatorio()
    {
        Validar(CommandValido() with { UserId = Guid.Empty })
            .ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode(CodigosErroConst.CampoObrigatorio);
    }

    [Fact]
    public void Validar_QuandoSemMidiasRemovidas_NaoDeveRetornarErros()
    {
        Validar(CommandValido() with { MidiasRemovidas = [] }).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_QuandoMidiaRemovidaComIdVazio_DeveRetornarErroComIndiceDaMidia()
    {
        var resultado = Validar(CommandValido() with { MidiasRemovidas = [Guid.NewGuid(), Guid.Empty] });

        resultado.ShouldHaveValidationErrorFor("MidiasRemovidas[1]").WithErrorCode(CodigosErroConst.CampoObrigatorio);
        resultado.ShouldNotHaveValidationErrorFor("MidiasRemovidas[0]");
    }
}
