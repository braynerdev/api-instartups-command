using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PostagemJaDesativadaException : ConflictException
{
    public PostagemJaDesativadaException()
        : base("A postagem já está desativada.")
    {
    }
}
