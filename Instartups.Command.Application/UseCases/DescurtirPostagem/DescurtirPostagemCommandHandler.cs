using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.Services;

namespace Instartups.Command.Application.UseCases.DescurtirPostagem;

public class DescurtirPostagemCommandHandler(
        IUnitOfWork unitOfWork
    ) : IVoidCommandHandler<DescurtirPostagemCommand>
{
    public async Task Handle(DescurtirPostagemCommand command, CancellationToken ct)
    {
        var perfil = await ObterPerfil(command.UserId, ct);
        var curtida = await ObterCurtida(command.PostagemId, perfil.Id, ct);

        CurtirPostagemService.Descurtir(curtida.Postagem, curtida.Postagem.Autor, perfil);

        unitOfWork.CurtidaPostagem.Remove(curtida);

        await unitOfWork.CommitAsync(ct);
    }

    private async Task<PerfilEntity> ObterPerfil(Guid usuarioId, CancellationToken ct)
    {
        var perfil = await unitOfWork.Perfil.ObterPorUsuarioIdAsync(usuarioId, ct);

        return perfil ?? throw new PerfilNaoEncontradoException();
    }

    private async Task<CurtidaPostagemEntity> ObterCurtida(Guid postagemId, Guid perfilId, CancellationToken ct)
    {
        var curtida = await unitOfWork.CurtidaPostagem.ObterComPostagemEAutorAsync(postagemId, perfilId, ct);

        return curtida ?? throw new PostagemNaoCurtidaException();
    }
}
