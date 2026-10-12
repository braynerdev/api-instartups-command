using FluentValidation;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Common.Validator;
using Instartups.Command.Tests.Common.Builders.Commands;

namespace Instartups.Command.Application.UnitTests.Common.Validators;

public class CadastrarPerfilCommandValidatorTests : PerfilCommandValidatorTestsBase<CadastrarPerfilCommand>
{
    protected override IValidator<CadastrarPerfilCommand> Validator { get; } = new CadastrarPerfilCommandValidator();

    protected override CadastrarPerfilCommand CommandValido() => PerfilCommandBuilder.CadastrarPerfil();
    protected override CadastrarPerfilCommand ComNome(string nome) => CommandValido() with { Nome = nome };
    protected override CadastrarPerfilCommand ComImagemPerfilUrl(string? url) => CommandValido() with { ImagemPerfilUrl = url };
    protected override CadastrarPerfilCommand ComImagemFundoUrl(string? url) => CommandValido() with { ImagemFundoUrl = url };
    protected override CadastrarPerfilCommand ComCoordenada(CoordenadaCommand coordenada) => CommandValido() with { Coordenada = coordenada };
}
