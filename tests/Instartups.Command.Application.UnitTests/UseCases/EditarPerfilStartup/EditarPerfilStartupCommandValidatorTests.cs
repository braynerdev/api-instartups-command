using FluentValidation;
using Instartups.Command.Application.UnitTests.Common.Validators;
using Instartups.Command.Application.UseCases.EditarPerfilStartup;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Tests.Common.Builders.Commands;

namespace Instartups.Command.Application.UnitTests.UseCases.EditarPerfilStartup;

public class EditarPerfilStartupCommandValidatorTests : StartupCommandValidatorTestsBase<EditarPerfilStartupCommand>
{
    protected override IValidator<EditarPerfilStartupCommand> Validator { get; } = new EditarPerfilStartupCommandValidator();

    protected override EditarPerfilStartupCommand CommandValido() => PerfilCommandBuilder.EditarStartup();

    protected override EditarPerfilStartupCommand ComNomePerfil(string nome)
    {
        var command = CommandValido();
        return command with { Perfil = command.Perfil with { Nome = nome } };
    }

    protected override EditarPerfilStartupCommand ComLatitudePerfil(double latitude)
    {
        var command = CommandValido();
        return command with { Perfil = command.Perfil with { Coordenada = command.Perfil.Coordenada with { Latitude = latitude } } };
    }

    protected override EditarPerfilStartupCommand SemPerfil() => CommandValido() with { Perfil = null! };

    protected override EditarPerfilStartupCommand ComPitch(string pitch) => CommandValido() with { Pitch = pitch };
    protected override EditarPerfilStartupCommand ComDataFundacao(DateOnly dataFundacao) => CommandValido() with { DataFundacao = dataFundacao };
    protected override EditarPerfilStartupCommand ComTamanhoEquipe(int tamanhoEquipe) => CommandValido() with { TamanhoEquipe = tamanhoEquipe };
    protected override EditarPerfilStartupCommand ComValorBuscado(decimal valorBuscado) => CommandValido() with { ValorBuscado = valorBuscado };
    protected override EditarPerfilStartupCommand ComSetor(SetorStartupEnum setor) => CommandValido() with { Setor = setor };
}
