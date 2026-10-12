using FluentValidation;
using Instartups.Command.Application.UnitTests.Common.Validators;
using Instartups.Command.Application.UseCases.CadastrarPerfilInvestidor;
using Instartups.Command.Tests.Common.Builders.Commands;

namespace Instartups.Command.Application.UnitTests.UseCases.CadastrarPerfilInvestidor;

public class CadastrarPerfilInvestidorCommandValidatorTests : InvestidorCommandValidatorTestsBase<CadastrarPerfilInvestidorCommand>
{
    protected override IValidator<CadastrarPerfilInvestidorCommand> Validator { get; } = new CadastrarPerfilInvestidorCommandValidator();

    protected override CadastrarPerfilInvestidorCommand CommandValido() => PerfilCommandBuilder.CadastrarInvestidor();

    protected override CadastrarPerfilInvestidorCommand ComNomePerfil(string nome)
    {
        var command = CommandValido();
        return command with { Perfil = command.Perfil with { Nome = nome } };
    }

    protected override CadastrarPerfilInvestidorCommand ComLatitudePerfil(double latitude)
    {
        var command = CommandValido();
        return command with { Perfil = command.Perfil with { Coordenada = command.Perfil.Coordenada with { Latitude = latitude } } };
    }

    protected override CadastrarPerfilInvestidorCommand SemPerfil() => CommandValido() with { Perfil = null! };

    protected override CadastrarPerfilInvestidorCommand ComTeseInvestimento(string tese) => CommandValido() with { TeseInvestimento = tese };
    protected override CadastrarPerfilInvestidorCommand ComTickets(decimal ticketMinimo, decimal ticketMaximo) => CommandValido() with { TicketMinimo = ticketMinimo, TicketMaximo = ticketMaximo };
}
