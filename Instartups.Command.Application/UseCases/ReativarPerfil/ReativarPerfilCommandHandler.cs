using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Application.UseCases.ReativarPerfil;

public class ReativarPerfilCommandHandler(
        IUnitOfWork unitOfWork
    ) : IVoidCommandHandler<ReativarPerfilCommand>
{
    public async Task Handle(ReativarPerfilCommand command, CancellationToken ct)
    {
        var perfil = await ObterPerfil(command.UserId, ct);

        ValidarPerfilDesativado(perfil);

        perfil.Ativar();

        await unitOfWork.CommitAsync(ct);
    }

    private async Task<PerfilEntity> ObterPerfil(Guid usuarioId, CancellationToken ct)
    {
        var perfil = await unitOfWork.Perfil.ObterPorUsuarioIdAsync(usuarioId, ct);

        return perfil ?? throw new PerfilNaoEncontradoException();
    }

    private static void ValidarPerfilDesativado(PerfilEntity perfil)
    {
        if (perfil.Ativo)
            throw new PerfilJaAtivoException();
    }
}
