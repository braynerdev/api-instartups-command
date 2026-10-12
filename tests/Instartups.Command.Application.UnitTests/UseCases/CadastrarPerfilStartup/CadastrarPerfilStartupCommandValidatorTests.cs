using FluentValidation;
using Instartups.Command.Application.UnitTests.Common.Validators;
using Instartups.Command.Application.UseCases.CadastrarPerfilStartup;
using Instartups.Command.Tests.Common.Builders.Commands;

namespace Instartups.Command.Application.UnitTests.UseCases.CadastrarPerfilStartup;

public class CadastrarPerfilStartupCommandValidatorTests : StartupCommandValidatorTestsBase<CadastrarPerfilStartupCommand>
{
    protected override IValidator<CadastrarPerfilStartupCommand> Validator { get; } = new CadastrarPerfilStartupCommandValidator();

    protected override CadastrarPerfilStartupCommand CommandValido() => PerfilCommandBuilder.CadastrarStartup();

    protected override CadastrarPerfilStartupCommand ComNomePerfil(string nome)
    {
        var command = CommandValido();
        return command with { Perfil = command.Perfil with { Nome = nome } };
    }

    protected override CadastrarPerfilStartupCommand ComLatitudePerfil(double latitude)
    {
        var command = CommandValido();
        return command with { Perfil = command.Perfil with { Coordenada = command.Perfil.Coordenada with { Latitude = latitude } } };
    }

    protected override CadastrarPerfilStartupCommand ComPitch(string pitch) => CommandValido() with { Pitch = pitch };
    protected override CadastrarPerfilStartupCommand ComDataFundacao(DateOnly dataFundacao) => CommandValido() with { DataFundacao = dataFundacao };
    protected override CadastrarPerfilStartupCommand ComTamanhoEquipe(int tamanhoEquipe) => CommandValido() with { TamanhoEquipe = tamanhoEquipe };
    protected override CadastrarPerfilStartupCommand ComValorBuscado(decimal valorBuscado) => CommandValido() with { ValorBuscado = valorBuscado };
}
