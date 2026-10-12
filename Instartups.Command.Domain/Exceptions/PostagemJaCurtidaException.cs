using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PostagemJaCurtidaException : ConflictException
{
    public PostagemJaCurtidaException()
        : base("Você já curtiu esta postagem.")
    {
    }
}
