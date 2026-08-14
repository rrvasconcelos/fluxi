namespace Fluxi.Domain.Exceptions;

public abstract class DomainException : Exception
{
    #region Constructors

    protected DomainException(string message)
        : base(message)
    {
    }

    #endregion
}