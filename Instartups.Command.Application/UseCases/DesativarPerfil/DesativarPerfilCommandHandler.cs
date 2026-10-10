using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Application.UseCases.DesativarPerfil;

public class DesativarPerfilCommandHandler(
        IUnitOfWork unitOfWork
    ) : IVoidCommandHandler<DesativarPerfilCommand>
{
    public async Task Handle(DesativarPerfilCommand command, CancellationToken ct)
    {
        var perfil = await ObterPerfil(command.UserId, ct);

        ValidarPerfilAtivo(perfil);

        perfil.Desativar();

        await unitOfWork.CommitAsync(ct);
    }

    private async Task<PerfilEntity> ObterPerfil(Guid usuarioId, CancellationToken ct)
    {
        var perfil = await unitOfWork.Perfil.ObterPorUsuarioIdAsync(usuarioId, ct);

        return perfil ?? throw new PerfilNaoEncontradoException();
    }

    private static void ValidarPerfilAtivo(PerfilEntity perfil)
    {
        if (!perfil.Ativo)
        {
            throw new PerfilJaDesativadoException();
        }
    }
}
