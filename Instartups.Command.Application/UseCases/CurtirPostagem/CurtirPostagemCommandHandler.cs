using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.Services;

namespace Instartups.Command.Application.UseCases.CurtirPostagem;

public class CurtirPostagemCommandHandler(
        IUnitOfWork unitOfWork
    ) : IVoidCommandHandler<CurtirPostagemCommand>
{
    public async Task Handle(CurtirPostagemCommand command, CancellationToken ct)
    {
        var perfil = await ObterPerfil(command.UserId, ct);
        var postagem = await ObterPostagem(command.PostagemId, ct);

        var jaCurtiu = await unitOfWork.CurtidaPostagem.JaCurtiuAsync(postagem.Id, perfil.Id, ct);

        var curtida = CurtirPostagemService.Curtir(postagem, postagem.Autor, perfil, jaCurtiu);

        unitOfWork.CurtidaPostagem.Add(curtida);

        await unitOfWork.CommitAsync(ct);
    }

    private async Task<PerfilEntity> ObterPerfil(Guid usuarioId, CancellationToken ct)
    {
        var perfil = await unitOfWork.Perfil.ObterPorUsuarioIdAsync(usuarioId, ct);

        return perfil ?? throw new PerfilNaoEncontradoException();
    }

    private async Task<PostagemEntity> ObterPostagem(Guid postagemId, CancellationToken ct)
    {
        var postagem = await unitOfWork.Postagem.ObterPorIdComAutorAsync(postagemId, ct);

        return postagem ?? throw new PostagemNaoEncontradaException();
    }
}
