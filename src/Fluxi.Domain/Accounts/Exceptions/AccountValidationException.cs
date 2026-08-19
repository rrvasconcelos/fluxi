using Fluxi.Domain.Exceptions;

namespace Fluxi.Domain.Accounts.Exceptions;

public sealed class AccountValidationException : DomainException
{
    #region Constructors

    public AccountValidationException(string message)
        : base(message)
    {
    }

    #endregion
}