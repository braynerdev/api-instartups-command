using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PostagemDesativadaException : DomainException
{
    public PostagemDesativadaException()
        : base("Não é possível realizar esta ação porque a postagem está desativada.")
    {
    }
}
