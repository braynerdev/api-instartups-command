using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.Services;

namespace Instartups.Command.Application.UseCases.DesativarPostagem;

public class DesativarPostagemCommandHandler(
        IUnitOfWork unitOfWork
    ) : IVoidCommandHandler<DesativarPostagemCommand>
{
    public async Task Handle(DesativarPostagemCommand command, CancellationToken ct)
    {
        var perfil = await ObterPerfil(command.UserId, ct);
        var postagem = await ObterPostagem(command.PostagemId, ct);

        DesativarPostagemService.Desativar(postagem, perfil);

        await unitOfWork.CommitAsync(ct);
    }

    private async Task<PerfilEntity> ObterPerfil(Guid usuarioId, CancellationToken ct)
    {
        var perfil = await unitOfWork.Perfil.ObterPorUsuarioIdAsync(usuarioId, ct);

        return perfil ?? throw new PerfilNaoEncontradoException();
    }

    private async Task<PostagemEntity> ObterPostagem(Guid postagemId, CancellationToken ct)
    {
        var postagem = await unitOfWork.Postagem.GetByIdAsync(postagemId, ct);

        return postagem ?? throw new PostagemNaoEncontradaException();
    }
}
