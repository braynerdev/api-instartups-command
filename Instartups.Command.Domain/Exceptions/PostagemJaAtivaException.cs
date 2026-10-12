using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PostagemJaAtivaException : ConflictException
{
    public PostagemJaAtivaException()
        : base("A postagem já está ativa.")
    {
    }
}
