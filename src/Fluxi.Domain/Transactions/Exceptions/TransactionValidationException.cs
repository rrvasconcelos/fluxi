using Fluxi.Domain.Exceptions;

namespace Fluxi.Domain.Transactions;

public sealed class TransactionValidationException : DomainException
{
    #region Constructors

    public TransactionValidationException(string message)
        : base(message)
    {
    }

    #endregion
}
