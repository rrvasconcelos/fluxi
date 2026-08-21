using Fluxi.Domain.Incomes.Exceptions;
using Fluxi.SharedKernel.Entities;

namespace Fluxi.Domain.Incomes.Entities;

public sealed class Income : AuditableEntity<Guid>
{
    #region Constructors

    private Income(
        Guid id,
        Guid accountId,
        string description,
        decimal amount,
        int receiptDay,
        bool isRecurring)
        : base(id)
    {
        AccountId = accountId;
        Description = description;
        Amount = amount;
        ReceiptDay = receiptDay;
        IsRecurring = isRecurring;
    }

    #endregion

    #region Properties

    public Guid AccountId { get; }

    public string Description { get; }

    public decimal Amount { get; }

    public int ReceiptDay { get; }

    public bool IsRecurring { get; }

    #endregion

    #region Factory Methods

    public static Income Create(
        Guid accountId,
        string description,
        decimal amount,
        int receiptDay,
        bool isRecurring = true)
    {
        Validate(accountId, description, amount, receiptDay);

        return new Income(
            Guid.NewGuid(),
            accountId,
            description.Trim(),
            amount,
            receiptDay,
            isRecurring);
    }

    #endregion

    #region Validation

    private static void Validate(
        Guid accountId,
        string description,
        decimal amount,
        int receiptDay)
    {
        if (accountId == Guid.Empty)
        {
            throw new IncomeValidationException("Income account is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new IncomeValidationException("Income description is required.");
        }

        if (amount <= 0)
        {
            throw new IncomeValidationException("Income amount must be greater than zero.");
        }

        if (receiptDay is < 1 or > 31)
        {
            throw new IncomeValidationException("Income receipt day must be between 1 and 31.");
        }
    }

    #endregion
}
