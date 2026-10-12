using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class MidiaPostagemNaoEncontradaException : NotFoundException
{
    public MidiaPostagemNaoEncontradaException()
        : base("Mídia da postagem não encontrada.")
    {
    }
}
