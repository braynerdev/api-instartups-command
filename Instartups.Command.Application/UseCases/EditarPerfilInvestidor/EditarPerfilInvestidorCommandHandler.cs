using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Application.UseCases.EditarPerfilInvestidor;

public class EditarPerfilInvestidorCommandHandler(
        IUnitOfWork unitOfWork
    ) : ICommandHandler<EditarPerfilInvestidorCommand, EditarPerfilInvestidorResponse>
{
    public async Task<EditarPerfilInvestidorResponse> Handle(EditarPerfilInvestidorCommand Command, CancellationToken ct)
    {
        var perfil = await ObterPerfilInvestidor(Command.Perfil.UserId, ct);

        EditarPerfilInvestidor(perfil, Command);
        EditarImagens(perfil, Command.Perfil);

        await unitOfWork.CommitAsync(ct);

        return EditarPerfilInvestidorResponse.FromEntity(perfil);
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
            teseInvestimento: command.TeseInvestimento,
            ticketMinimo: command.TicketMinimo,
            ticketMaximo: command.TicketMaximo
        );
    }

    private static void EditarImagens(PerfilEntity perfil, EditarPerfilCommand command)
    {
        perfil.SetImagemPerfilUrl(command.ImagemPerfilUrl);
        perfil.SetImagemFundoUrl(command.ImagemFundoUrl);
    }
}
