using Fluxi.Domain.Exceptions;

namespace Fluxi.Domain.FixedExpenses.Exceptions;

public sealed class FixedExpenseValidationException : DomainException
{
    #region Constructors

    public FixedExpenseValidationException(string message)
        : base(message)
    {
    }

    #endregion
}
