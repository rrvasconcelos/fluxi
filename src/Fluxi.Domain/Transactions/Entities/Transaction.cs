using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Fluxi.SharedKernel.Entities;

namespace Fluxi.Domain.Transactions;

public sealed class Transaction : Entity
{
    #region Constructors

    private Transaction(
        Guid id,
        Guid accountId,
        DateOnly date,
        string description,
        decimal amount,
        TransactionType type,
        TransactionOrigin origin,
        Guid? categoryId,
        Guid? invoiceId)
        : base(id)
    {
        AccountId = accountId;
        Date = date;
        Description = description;
        Amount = amount;
        Type = type;
        Origin = origin;
        CategoryId = categoryId;
        InvoiceId = invoiceId;
        Hash = CalculateHash(accountId, date, amount, description);
    }

    #endregion

    #region Properties

    public Guid AccountId { get; }

    public Guid? InvoiceId { get; }

    public Guid? CategoryId { get; private set; }

    public DateOnly Date { get; }

    public string Description { get; }

    public decimal Amount { get; }

    public TransactionType Type { get; }

    public TransactionOrigin Origin { get; }

    public string Hash { get; }

    #endregion

    #region Factory Methods

    public static Transaction Create(
        Guid accountId,
        DateOnly date,
        string description,
        decimal amount,
        TransactionType type,
        TransactionOrigin origin,
        Guid? categoryId = null,
        Guid? invoiceId = null)
    {
        Validate(accountId, description, amount, type, origin, categoryId, invoiceId);

        return new Transaction(
            Guid.NewGuid(),
            accountId,
            date,
            description.Trim(),
            amount,
            type,
            origin,
            categoryId,
            invoiceId);
    }

    #endregion

    #region Methods

    public void AssignCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new TransactionValidationException("Transaction category is required.");
        }

        CategoryId = categoryId;
    }

    #endregion

    #region Validation

    private static void Validate(
        Guid accountId,
        string description,
        decimal amount,
        TransactionType type,
        TransactionOrigin origin,
        Guid? categoryId,
        Guid? invoiceId)
    {
        if (accountId == Guid.Empty)
        {
            throw new TransactionValidationException("Transaction account is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new TransactionValidationException("Transaction description is required.");
        }

        if (amount <= 0)
        {
            throw new TransactionValidationException("Transaction amount must be greater than zero.");
        }

        if (!Enum.IsDefined(type))
        {
            throw new TransactionValidationException("Transaction type is not supported.");
        }

        if (!Enum.IsDefined(origin))
        {
            throw new TransactionValidationException("Transaction origin is not supported.");
        }

        if (categoryId == Guid.Empty)
        {
            throw new TransactionValidationException("Transaction category is required.");
        }

        if (invoiceId == Guid.Empty)
        {
            throw new TransactionValidationException("Transaction invoice is required.");
        }
    }

    private static string CalculateHash(
        Guid accountId,
        DateOnly date,
        decimal amount,
        string description)
    {
        string input = string.Join(
            "|",
            accountId.ToString("N"),
            date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            amount.ToString("0.############################", CultureInfo.InvariantCulture),
            description);

        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    #endregion
}
