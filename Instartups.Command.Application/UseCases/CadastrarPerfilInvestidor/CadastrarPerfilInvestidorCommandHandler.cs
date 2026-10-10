using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Application.UseCases.CadastrarPerfilInvestidor;

public class CadastrarPerfilInvestidorCommandHandler(
        IUnitOfWork unitOfWork
    ) : ICommandHandler<CadastrarPerfilInvestidorCommand, CadastrarPerfilInvestidorResponse>
{
    public async Task<CadastrarPerfilInvestidorResponse> Handle(CadastrarPerfilInvestidorCommand Command, CancellationToken ct)
    {
        await ValidarUsuarioJaPossuiPerfil(Command.Perfil.UserId, ct);

        var perfil = CriarPerfilInvestidor(Command);
        unitOfWork.Perfil.Add(perfil);

        perfil.EditarImagemPerfilUrl(Command.Perfil.ImagemPerfilUrl);
        perfil.EditarImagemFundoUrl(Command.Perfil.ImagemFundoUrl);

        await unitOfWork.CommitAsync(ct);

        return CadastrarPerfilInvestidorResponse.FromEntity(perfil, Command.TeseInvestimento, Command.TicketMinimo, Command.TicketMaximo);
    }

    private async Task ValidarUsuarioJaPossuiPerfil(Guid usuarioId, CancellationToken ct)
    {
        var perfilExistente = await unitOfWork.Perfil.UsuarioJaPossuiPerfilAsync(usuarioId, ct);

        if (perfilExistente)
        {
            throw new UsuarioJaPossuiPerfilException();
        }
    }

    private PerfilEntity CriarPerfilInvestidor(CadastrarPerfilInvestidorCommand command)
    {
        return PerfilEntity.CriarInvestidor(
                nome: command.Perfil.Nome,
                latitude: command.Perfil.Coordenada.Latitude,
                longitude: command.Perfil.Coordenada.Longitude,
                usuarioId: command.Perfil.UserId,
                teseInvestimento: command.TeseInvestimento,
                ticketMinimo: command.TicketMinimo,
                ticketMaximo: command.TicketMaximo
        );
    }
}
