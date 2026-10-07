namespace Instartups.Command.Domain.Exceptions.Base;

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}