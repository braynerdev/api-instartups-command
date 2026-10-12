using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Application.UseCases.EditarPerfilInvestidor;

public class EditarPerfilInvestidorCommandHandler(
        IUnitOfWork unitOfWork
    ) : IVoidCommandHandler<EditarPerfilInvestidorCommand>
{
    public async Task Handle(EditarPerfilInvestidorCommand command, CancellationToken ct)
    {
        var perfil = await ObterPerfilInvestidor(command.Perfil.UserId, ct);

        EditarPerfilInvestidor(perfil, command);

        await unitOfWork.CommitAsync(ct);
    }

    private async Task<PerfilEntity> ObterPerfilInvestidor(Guid usuarioId, CancellationToken ct)
    {
        var perfil = await unitOfWork.Perfil.ObterPorUsuarioIdComInvestidorAsync(usuarioId, ct);

        return perfil ?? throw new PerfilNaoEncontradoException();
    }

    private static void EditarPerfilInvestidor(PerfilEntity perfil, EditarPerfilInvestidorCommand command)
    {
        perfil.EditarInvestidor(
            nome: command.Perfil.Nome,
            latitude: command.Perfil.Coordenada.Latitude,
            longitude: command.Perfil.Coordenada.Longitude,
            imagemPerfilUrl: command.Perfil.ImagemPerfilUrl,
            imagemFundoUrl: command.Perfil.ImagemFundoUrl,
            teseInvestimento: command.TeseInvestimento,
            ticketMinimo: command.TicketMinimo,
            ticketMaximo: command.TicketMaximo
        );
    }
}
