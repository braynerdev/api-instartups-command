using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PerfilNaoPodeSeguirASiMesmoException : DomainException
{
    public PerfilNaoPodeSeguirASiMesmoException()
        : base("Um perfil não pode seguir a si mesmo.")
    {
    }
}
