using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PostagemNaoEncontradaException : NotFoundException
{
    public PostagemNaoEncontradaException()
        : base("Postagem não encontrada.")
    {
    }
}
