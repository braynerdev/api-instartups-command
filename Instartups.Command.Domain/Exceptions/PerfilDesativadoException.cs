using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PerfilDesativadoException : DomainException
{
    public PerfilDesativadoException()
        : base("Não é possível realizar esta ação porque o perfil está desativado.")
    {

    }
}
