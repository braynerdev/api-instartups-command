using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class UsuarioJaPossuiPerfilException : ConflictException
{
    public UsuarioJaPossuiPerfilException()
        : base("O usuário já possui um perfil cadastrado.")
    {
    }
}
