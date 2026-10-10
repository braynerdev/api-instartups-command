using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Application.UseCases.EditarCoordenadaPerfil;

public class EditarCoordenadaPerfilCommandHandler(
        IUnitOfWork unitOfWork
    ) : IVoidCommandHandler<EditarCoordenadaPerfilCommand>
{
    public async Task Handle(EditarCoordenadaPerfilCommand command, CancellationToken ct)
    {
        var perfil = await ObterPerfil(command.UserId, ct);

        perfil.EditarCoordenadas(command.Coordenada.Latitude, command.Coordenada.Longitude);

        await unitOfWork.CommitAsync(ct);
    }

    private async Task<PerfilEntity> ObterPerfil(Guid usuarioId, CancellationToken ct)
    {
        var perfil = await unitOfWork.Perfil.ObterPorUsuarioIdAsync(usuarioId, ct);

        return perfil ?? throw new PerfilNaoEncontradoException();
    }
}
