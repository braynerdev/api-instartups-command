using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.Services;

namespace Instartups.Command.Application.UseCases.SeguirPerfil;

public class SeguirPerfilCommandHandler(
        IUnitOfWork unitOfWork
    ) : IVoidCommandHandler<SeguirPerfilCommand>
{
    public async Task Handle(SeguirPerfilCommand command, CancellationToken ct)
    {
        var seguidor = await ObterPerfil(command.UserId, ct);
        var seguido = await ObterPerfilSeguido(command.SeguidoId, ct);

        var jaSegue = await unitOfWork.PerfilSeguidor.JaSegueAsync(seguidor.Id, seguido.Id, ct);

        var perfilSeguidor = SeguirPerfilService.Seguir(seguidor, seguido, jaSegue);

        unitOfWork.PerfilSeguidor.Add(perfilSeguidor);

        await unitOfWork.CommitAsync(ct);
    }

    private async Task<PerfilEntity> ObterPerfil(Guid usuarioId, CancellationToken ct)
    {
        var perfil = await unitOfWork.Perfil.ObterPorUsuarioIdAsync(usuarioId, ct);

        return perfil ?? throw new PerfilNaoEncontradoException();
    }

    private async Task<PerfilEntity> ObterPerfilSeguido(Guid seguidoId, CancellationToken ct)
    {
        var perfil = await unitOfWork.Perfil.GetByIdAsync(seguidoId, ct);

        return perfil ?? throw new PerfilNaoEncontradoException();
    }
}
