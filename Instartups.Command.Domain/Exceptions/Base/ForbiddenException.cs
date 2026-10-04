namespace Instartups.Command.Domain.Exceptions.Base;

public class ForbiddenException : BaseException
{
    public ForbiddenException(string message)
        : base(message)
    {
    }
}