using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PerfilNaoPodeCurtirPropriaPostagemException : DomainException
{
    public PerfilNaoPodeCurtirPropriaPostagemException()
        : base("Um perfil não pode curtir a própria postagem.")
    {
    }
}
