using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Application.UseCases.EditarPerfilStartup;

public class EditarPerfilStartupCommandHandler(
        IUnitOfWork unitOfWork
    ) : IVoidCommandHandler<EditarPerfilStartupCommand>
{
    public async Task Handle(EditarPerfilStartupCommand command, CancellationToken ct)
    {
        var perfil = await ObterPerfilStartup(command.Perfil.UserId, ct);

        EditarPerfilStartup(perfil, command);

        await unitOfWork.CommitAsync(ct);
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
            imagemPerfilUrl: command.Perfil.ImagemPerfilUrl,
            imagemFundoUrl: command.Perfil.ImagemFundoUrl,
            pitch: command.Pitch,
            dataFundacao: command.DataFundacao,
            tamanhoEquipe: command.TamanhoEquipe,
            valorBuscado: command.ValorBuscado
        );
    }
}
