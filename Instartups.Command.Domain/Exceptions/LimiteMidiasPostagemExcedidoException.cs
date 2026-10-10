using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class LimiteMidiasPostagemExcedidoException : DomainException
{
    public LimiteMidiasPostagemExcedidoException(int limite)
        : base($"Não é possível adicionar mais de {limite} mídias a uma postagem.")
    {
    }
}
