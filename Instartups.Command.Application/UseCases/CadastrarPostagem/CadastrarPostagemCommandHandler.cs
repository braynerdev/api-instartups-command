using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.Common.Response;
using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Application.UseCases.CadastrarPostagem;

public class CadastrarPostagemCommandHandler(
    IUnitOfWork UnitOfWork) : ICommandHandler<CadastrarPostagemCommand,IdResponse>
{
    public async Task<IdResponse> Handle(CadastrarPostagemCommand command, CancellationToken ct)
    {
        var perfil = await PegarPerfilAuthor(command.AutorId, ct);
        
        ValidarAutor(perfil);

        var postagem = PostagemEntity.Criar(command.Descricao, perfil!.Id);
        AdicionarMidiasUrl(postagem, command.Midias);
        
        UnitOfWork.Postagem.Add(postagem);
        
        await UnitOfWork.CommitAsync(ct);
        
        return new IdResponse(postagem.Id);
    }

    private async Task<PerfilEntity?> PegarPerfilAuthor(Guid autorId,  CancellationToken ct)
    {
        return await UnitOfWork.Perfil.ObterPorUsuarioIdAsync(autorId,  ct);
    }

    private void ValidarAutor(PerfilEntity? perfil)
    {
        if (perfil == null)
            throw new PerfilNaoEncontradoException();
        
        if (!perfil.Ativo)
            throw new PerfilDesativadoException();
    }

    private void AdicionarMidiasUrl(PostagemEntity postagem, IEnumerable<MidiaCommand> midias)
    {
        foreach (var midia in midias)
        {
            postagem.AdicionarMidiaPostagem(midia.Url, midia.Tipo);
        }
    }
}