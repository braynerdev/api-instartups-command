using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PerfilNaoEhAutorPostagemException : ForbiddenException
{
    public PerfilNaoEhAutorPostagemException()
        : base("Apenas o autor pode editar a postagem.")
    {
    }
}
