using FluentValidation;
using FluentValidation.TestHelper;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.UnitTests.Common.Validators;
using Instartups.Command.Application.UseCases.CadastrarPostagem;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Tests.Common.Builders.Commands;

namespace Instartups.Command.Application.UnitTests.UseCases.CadastrarPostagem;

public class CadastrarPostagemCommandValidatorTests : PostagemCommandValidatorTestsBase<CadastrarPostagemCommand>
{
    protected override IValidator<CadastrarPostagemCommand> Validator { get; } = new CadastrarPostagemCommandValidator();
    protected override string PropriedadeMidias => nameof(CadastrarPostagemCommand.Midias);

    protected override CadastrarPostagemCommand CommandValido() => PostagemCommandBuilder.CadastrarPostagem();
    protected override CadastrarPostagemCommand ComDescricao(string descricao) => CommandValido() with { Descricao = descricao };
    protected override CadastrarPostagemCommand ComMidias(IEnumerable<MidiaCommand> midias) => CommandValido() with { Midias = midias };

    [Fact]
    public void Validar_QuandoAutorIdVazio_DeveRetornarErroCampoObrigatorio()
    {
        Validar(CommandValido() with { AutorId = Guid.Empty })
            .ShouldHaveValidationErrorFor(x => x.AutorId)
            .WithErrorCode(CodigosErroConst.CampoObrigatorio);
    }
}
