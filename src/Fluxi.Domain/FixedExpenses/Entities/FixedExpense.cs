using Fluxi.Domain.FixedExpenses.Exceptions;
using Fluxi.SharedKernel.Entities;

namespace Fluxi.Domain.FixedExpenses.Entities;

public sealed class FixedExpense : AuditableEntity<Guid>
{
    #region Constructors

    private FixedExpense(
        Guid id,
        string description,
        decimal amount,
        int dueDay,
        string? notes)
        : base(id)
    {
        Description = description;
        Amount = amount;
        DueDay = dueDay;
        Notes = notes;
    }

    #endregion

    #region Properties

    public string Description { get; }

    public decimal Amount { get; }

    public int DueDay { get; }

    public string? Notes { get; }

    #endregion

    #region Factory Methods

    public static FixedExpense Create(
        string description,
        decimal amount,
        int dueDay,
        string? notes = null)
    {
        Validate(description, amount, dueDay);

        return new FixedExpense(
            Guid.NewGuid(),
            description.Trim(),
            amount,
            dueDay,
            notes?.Trim());
    }

    #endregion

    #region Validation

    private static void Validate(
        string description,
        decimal amount,
        int dueDay)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new FixedExpenseValidationException("Fixed expense description is required.");
        }

        if (amount <= 0)
        {
            throw new FixedExpenseValidationException("Fixed expense amount must be greater than zero.");
        }

        if (dueDay is < 1 or > 31)
        {
            throw new FixedExpenseValidationException("Fixed expense due day must be between 1 and 31.");
        }
    }

    #endregion
}
