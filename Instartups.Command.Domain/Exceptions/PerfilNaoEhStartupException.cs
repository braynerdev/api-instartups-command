using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PerfilNaoEhStartupException : DomainException
{
    public PerfilNaoEhStartupException()
        : base("O perfil não é do tipo STARTUP.")
    {
    }
}
