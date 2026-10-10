using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Application.UseCases.EditarPerfilStartup;

public class EditarPerfilStartupCommandHandler(
        IUnitOfWork unitOfWork
    ) : ICommandHandler<EditarPerfilStartupCommand, EditarPerfilStartupResponse>
{
    public async Task<EditarPerfilStartupResponse> Handle(EditarPerfilStartupCommand command, CancellationToken ct)
    {
        var perfil = await ObterPerfilStartup(command.Perfil.UserId, ct);

        EditarPerfilStartup(perfil, command);
        EditarImagens(perfil, command.Perfil);

        await unitOfWork.CommitAsync(ct);

        return EditarPerfilStartupResponse.FromEntity(perfil);
    }

    private async Task<PerfilEntity> ObterPerfilStartup(Guid usuarioId, CancellationToken ct)
    {
        var perfil = await unitOfWork.Perfil.ObterPorUsuarioIdComStartupAsync(usuarioId, ct);

        return perfil ?? throw new PerfilNaoEncontradoException();
    }

    private static void EditarPerfilStartup(PerfilEntity perfil, EditarPerfilStartupCommand command)
    {
        perfil.EditarStartup(
            nome: command.Perfil.Nome,
            latitude: command.Perfil.Coordenada.Latitude,
            longitude: command.Perfil.Coordenada.Longitude,
            pitch: command.Pitch,
            dataFundacao: command.DataFundacao,
            tamanhoEquipe: command.TamanhoEquipe,
            valorBuscado: command.ValorBuscado
        );
    }

    private static void EditarImagens(PerfilEntity perfil, EditarPerfilCommand command)
    {
        perfil.SetImagemPerfilUrl(command.ImagemPerfilUrl);
        perfil.SetImagemFundoUrl(command.ImagemFundoUrl);
    }
}
