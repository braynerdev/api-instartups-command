using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class UsuarioNaoIdentificadoException : UnauthorizedException
{
    public UsuarioNaoIdentificadoException()
        : base("Não foi possível identificar o usuário autenticado.")
    {
    }
}
