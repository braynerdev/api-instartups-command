using FluentValidation;
using Instartups.Command.Application.UnitTests.Common.Validators;
using Instartups.Command.Application.UseCases.EditarPerfilInvestidor;
using Instartups.Command.Tests.Common.Builders.Commands;

namespace Instartups.Command.Application.UnitTests.UseCases.EditarPerfilInvestidor;

public class EditarPerfilInvestidorCommandValidatorTests : InvestidorCommandValidatorTestsBase<EditarPerfilInvestidorCommand>
{
    protected override IValidator<EditarPerfilInvestidorCommand> Validator { get; } = new EditarPerfilInvestidorCommandValidator();

    protected override EditarPerfilInvestidorCommand CommandValido() => PerfilCommandBuilder.EditarInvestidor();

    protected override EditarPerfilInvestidorCommand ComNomePerfil(string nome)
    {
        var command = CommandValido();
        return command with { Perfil = command.Perfil with { Nome = nome } };
    }

    protected override EditarPerfilInvestidorCommand ComLatitudePerfil(double latitude)
    {
        var command = CommandValido();
        return command with { Perfil = command.Perfil with { Coordenada = command.Perfil.Coordenada with { Latitude = latitude } } };
    }

    protected override EditarPerfilInvestidorCommand SemPerfil() => CommandValido() with { Perfil = null! };

    protected override EditarPerfilInvestidorCommand ComTeseInvestimento(string tese) => CommandValido() with { TeseInvestimento = tese };
    protected override EditarPerfilInvestidorCommand ComTickets(decimal ticketMinimo, decimal ticketMaximo) => CommandValido() with { TicketMinimo = ticketMinimo, TicketMaximo = ticketMaximo };
}
