using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PerfilJaAtivoException : ConflictException
{
    public PerfilJaAtivoException()
        : base("O perfil já está ativo.")
    {
    }
}
