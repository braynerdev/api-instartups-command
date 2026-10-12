using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PerfilNaoEncontradoException : NotFoundException
{
    public PerfilNaoEncontradoException()
        : base("Perfil não encontrado.")
    {
    }
}
