using Fluxi.Domain.Exceptions;

namespace Fluxi.Domain.Invoices.Exceptions;

public sealed class InvoiceValidationException : DomainException
{
    #region Constructors

    public InvoiceValidationException(string message)
        : base(message)
    {
    }

    #endregion
}
