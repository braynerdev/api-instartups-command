using FluentValidation;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Common.Validator;
using Instartups.Command.Tests.Common.Builders.Commands;

namespace Instartups.Command.Application.UnitTests.Common.Validators;

public class EditarPerfilCommandValidatorTests : PerfilCommandValidatorTestsBase<EditarPerfilCommand>
{
    protected override IValidator<EditarPerfilCommand> Validator { get; } = new EditarPerfilCommandValidator();

    protected override EditarPerfilCommand CommandValido() => PerfilCommandBuilder.EditarPerfil();
    protected override EditarPerfilCommand ComNome(string nome) => CommandValido() with { Nome = nome };
    protected override EditarPerfilCommand ComImagemPerfilUrl(string? url) => CommandValido() with { ImagemPerfilUrl = url };
    protected override EditarPerfilCommand ComImagemFundoUrl(string? url) => CommandValido() with { ImagemFundoUrl = url };
    protected override EditarPerfilCommand ComCoordenada(CoordenadaCommand coordenada) => CommandValido() with { Coordenada = coordenada };
}
