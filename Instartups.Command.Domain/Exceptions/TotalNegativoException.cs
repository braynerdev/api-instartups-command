using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class TotalNegativoException : DomainException
{
    public TotalNegativoException()
        : base("O total não pode ser negativo.")
    {
    }
}
