namespace Instartups.Command.Domain.Exceptions.Base;

public abstract class ValidationException : BaseException
{
    protected ValidationException(string message) : base(message)
    {
    }
}