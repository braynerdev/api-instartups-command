using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Application.UseCases.DeixarDeSeguirPerfil;

public class DeixarDeSeguirPerfilCommandHandler(
        IUnitOfWork unitOfWork
    ) : IVoidCommandHandler<DeixarDeSeguirPerfilCommand>
{
    public async Task Handle(DeixarDeSeguirPerfilCommand command, CancellationToken ct)
    {
        var seguidor = await ObterPerfil(command.UserId, ct);
        var perfilSeguidor = await ObterPerfilSeguidor(seguidor.Id, command.SeguidoId, ct);

        seguidor.DeixarDeSeguir(perfilSeguidor.Seguido);

        unitOfWork.PerfilSeguidor.Remove(perfilSeguidor);

        await unitOfWork.CommitAsync(ct);
    }

    private async Task<PerfilEntity> ObterPerfil(Guid usuarioId, CancellationToken ct)
    {
        var perfil = await unitOfWork.Perfil.ObterPorUsuarioIdAsync(usuarioId, ct);

        return perfil ?? throw new PerfilNaoEncontradoException();
    }

    private async Task<PerfilSeguidorEntity> ObterPerfilSeguidor(Guid seguidorId, Guid seguidoId, CancellationToken ct)
    {
        var perfilSeguidor = await unitOfWork.PerfilSeguidor.ObterComSeguidoAsync(seguidorId, seguidoId, ct);

        return perfilSeguidor ?? throw new PerfilNaoSeguidoException();
    }
}
