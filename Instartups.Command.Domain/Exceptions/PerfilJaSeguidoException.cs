using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PerfilJaSeguidoException : ConflictException
{
    public PerfilJaSeguidoException()
        : base("Você já segue este perfil.")
    {
    }
}
