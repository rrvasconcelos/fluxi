using Fluxi.Domain.Invoices.Enums;
using Fluxi.Domain.Invoices.Exceptions;
using Fluxi.SharedKernel.Entities;

namespace Fluxi.Domain.Invoices.Entities;

public sealed class Invoice : AuditableEntity<Guid>
{
    #region Constructors

    private Invoice(
        Guid id,
        Guid accountId,
        string reference,
        DateOnly closingDate,
        DateOnly dueDate)
        : base(id)
    {
        AccountId = accountId;
        Reference = reference;
        ClosingDate = closingDate;
        DueDate = dueDate;
        Status = InvoiceStatus.Open;
    }

    #endregion

    #region Properties

    public Guid AccountId { get; }

    public string Reference { get; }

    public DateOnly ClosingDate { get; }

    public DateOnly DueDate { get; }

    public decimal? TotalAmount { get; private set; }

    public InvoiceStatus Status { get; private set; }

    #endregion

    #region Factory Methods

    public static Invoice Create(
        Guid accountId,
        string reference,
        DateOnly closingDate,
        DateOnly dueDate)
    {
        Validate(accountId, reference, closingDate, dueDate);

        return new Invoice(Guid.NewGuid(), accountId, reference.Trim(), closingDate, dueDate);
    }

    #endregion

    #region Methods

    public void Close()
    {
        if (Status != InvoiceStatus.Open)
        {
            throw new InvoiceValidationException("Only an open invoice can be closed.");
        }

        Status = InvoiceStatus.Closed;
    }

    public void Pay()
    {
        if (Status != InvoiceStatus.Closed)
        {
            throw new InvoiceValidationException("Only a closed invoice can be paid.");
        }

        Status = InvoiceStatus.Paid;
    }

    public void UpdateTotalAmount(decimal totalAmount)
    {
        if (totalAmount < 0)
        {
            throw new InvoiceValidationException("Invoice total amount cannot be negative.");
        }

        TotalAmount = totalAmount;
    }

    #endregion

    #region Validation

    private static void Validate(
        Guid accountId,
        string reference,
        DateOnly closingDate,
        DateOnly dueDate)
    {
        if (accountId == Guid.Empty)
        {
            throw new InvoiceValidationException("Invoice account is required.");
        }

        if (string.IsNullOrWhiteSpace(reference))
        {
            throw new InvoiceValidationException("Invoice reference is required.");
        }

        if (closingDate == default)
        {
            throw new InvoiceValidationException("Invoice closing date is required.");
        }

        if (dueDate == default)
        {
            throw new InvoiceValidationException("Invoice due date is required.");
        }

        if (dueDate < closingDate)
        {
            throw new InvoiceValidationException("Invoice due date cannot be before the closing date.");
        }
    }

    #endregion
}

