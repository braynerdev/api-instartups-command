using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PerfilJaDesativadoException : ConflictException
{
    public PerfilJaDesativadoException()
        : base("O perfil já está desativado.")
    {
    }
}
