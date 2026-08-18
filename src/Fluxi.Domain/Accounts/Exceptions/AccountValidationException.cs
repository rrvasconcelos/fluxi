using Fluxi.Domain.Exceptions;

namespace Fluxi.Domain.Accounts;

public sealed class AccountValidationException : DomainException
{
    #region Constructors

    public AccountValidationException(string message)
        : base(message)
    {
    }

    #endregion
}