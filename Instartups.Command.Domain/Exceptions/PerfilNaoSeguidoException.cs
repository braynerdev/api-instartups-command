using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PerfilNaoSeguidoException : NotFoundException
{
    public PerfilNaoSeguidoException()
        : base("Você não segue este perfil.")
    {
    }
}
