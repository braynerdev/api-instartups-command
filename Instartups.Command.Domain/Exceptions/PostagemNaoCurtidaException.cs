using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PostagemNaoCurtidaException : NotFoundException
{
    public PostagemNaoCurtidaException()
        : base("Você não curtiu esta postagem.")
    {
    }
}
