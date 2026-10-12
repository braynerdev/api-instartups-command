using Instartups.Command.Domain.Exceptions.Base;

namespace Instartups.Command.Domain.Exceptions;

public class PerfilNaoEhInvestidorException : DomainException
{
    public PerfilNaoEhInvestidorException()
        : base("O perfil não é do tipo INVESTIDOR.")
    {
    }
}
