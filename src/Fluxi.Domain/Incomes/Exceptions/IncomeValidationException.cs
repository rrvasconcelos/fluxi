using Fluxi.Domain.Exceptions;

namespace Fluxi.Domain.Incomes.Exceptions;

public sealed class IncomeValidationException : DomainException
{
    #region Constructors

    public IncomeValidationException(string message)
        : base(message)
    {
    }

    #endregion
}
