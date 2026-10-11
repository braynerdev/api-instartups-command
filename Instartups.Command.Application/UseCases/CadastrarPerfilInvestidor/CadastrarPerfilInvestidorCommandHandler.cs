using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Application.UseCases.CadastrarPerfilInvestidor;

public class CadastrarPerfilInvestidorCommandHandler(
        IUnitOfWork unitOfWork
    ) : ICommandHandler<CadastrarPerfilInvestidorCommand, CadastrarPerfilInvestidorResponse>
{
    public async Task<CadastrarPerfilInvestidorResponse> Handle(CadastrarPerfilInvestidorCommand command, CancellationToken ct)
    {
        await ValidarUsuarioJaPossuiPerfil(command.Perfil.UserId, ct);

        var perfil = CriarPerfilInvestidor(command);

        unitOfWork.Perfil.Add(perfil);

        await unitOfWork.CommitAsync(ct);

        return CadastrarPerfilInvestidorResponse.FromEntity(perfil, command.TeseInvestimento, command.TicketMinimo, command.TicketMaximo);
    }

    private async Task ValidarUsuarioJaPossuiPerfil(Guid usuarioId, CancellationToken ct)
    {
        var perfilExistente = await unitOfWork.Perfil.UsuarioJaPossuiPerfilAsync(usuarioId, ct);

        if (perfilExistente)
            throw new UsuarioJaPossuiPerfilException();
    }

    private PerfilEntity CriarPerfilInvestidor(CadastrarPerfilInvestidorCommand command)
    {
        return PerfilEntity.CriarInvestidor(
                nome: command.Perfil.Nome,
                latitude: command.Perfil.Coordenada.Latitude,
                longitude: command.Perfil.Coordenada.Longitude,
                usuarioId: command.Perfil.UserId,
                imagemPerfilUrl: command.Perfil.ImagemPerfilUrl,
                imagemFundoUrl: command.Perfil.ImagemFundoUrl,
                teseInvestimento: command.TeseInvestimento,
                ticketMinimo: command.TicketMinimo,
                ticketMaximo: command.TicketMaximo
        );
    }
}
