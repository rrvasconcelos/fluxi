using Fluxi.Domain.Transactions.Entities;
using Fluxi.Domain.Transactions.Enums;
using Fluxi.Domain.Transactions.Exceptions;

namespace Fluxi.Domain.Tests.Transactions;

public sealed class TransactionTests
{
    #region Tests

    [Fact]
    public void Create_WithValidData_ShouldCreateTransaction()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        DateOnly date = new(2026, 8, 17);
        const string description = "Grocery store";
        const decimal amount = 125.50m;

        // Act
        Transaction transaction = Transaction.Create(
            accountId,
            date,
            description,
            amount,
            TransactionType.Expense,
            TransactionOrigin.Manual);

        // Assert
        Assert.NotEqual(Guid.Empty, transaction.Id);
        Assert.Equal(accountId, transaction.AccountId);
        Assert.Equal(date, transaction.Date);
        Assert.Equal(description, transaction.Description);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal(TransactionType.Expense, transaction.Type);
        Assert.Equal(TransactionOrigin.Manual, transaction.Origin);
        Assert.Null(transaction.CategoryId);
        Assert.Null(transaction.InvoiceId);
        Assert.False(string.IsNullOrWhiteSpace(transaction.Hash));
    }

    [Fact]
    public void Create_ShouldTrimDescription()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();

        // Act
        Transaction transaction = Transaction.Create(
            accountId,
            new DateOnly(2026, 8, 17),
            "  Salary  ",
            5000m,
            TransactionType.Income,
            TransactionOrigin.Imported);

        // Assert
        Assert.Equal("Salary", transaction.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidDescription_ShouldThrow(string? description)
    {
        // Arrange
        Action action = () => Transaction.Create(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 17),
            description!,
            10m,
            TransactionType.Expense,
            TransactionOrigin.Manual);

        // Act
        TransactionValidationException exception = Assert.Throws<TransactionValidationException>(action);

        // Assert
        Assert.Equal("Transaction description is required.", exception.Message);
    }

    [Fact]
    public void Create_WithEmptyAccountId_ShouldThrow()
    {
        // Arrange
        Action action = () => Transaction.Create(
            Guid.Empty,
            new DateOnly(2026, 8, 17),
            "Purchase",
            10m,
            TransactionType.Expense,
            TransactionOrigin.Manual);

        // Act
        TransactionValidationException exception = Assert.Throws<TransactionValidationException>(action);

        // Assert
        Assert.Equal("Transaction account is required.", exception.Message);
    }

    [Fact]
    public void Create_WithNonPositiveAmount_ShouldThrow()
    {
        // Arrange
        Action action = () => Transaction.Create(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 17),
            "Purchase",
            0m,
            TransactionType.Expense,
            TransactionOrigin.Manual);

        // Act
        TransactionValidationException exception = Assert.Throws<TransactionValidationException>(action);

        // Assert
        Assert.Equal("Transaction amount must be greater than zero.", exception.Message);
    }

    [Fact]
    public void Create_WithUnsupportedType_ShouldThrow()
    {
        // Arrange
        Action action = () => Transaction.Create(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 17),
            "Purchase",
            10m,
            (TransactionType)999,
            TransactionOrigin.Manual);

        // Act
        TransactionValidationException exception = Assert.Throws<TransactionValidationException>(action);

        // Assert
        Assert.Equal("Transaction type is not supported.", exception.Message);
    }

    [Fact]
    public void Create_WithUnsupportedOrigin_ShouldThrow()
    {
        // Arrange
        Action action = () => Transaction.Create(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 17),
            "Purchase",
            10m,
            TransactionType.Expense,
            (TransactionOrigin)999);

        // Act
        TransactionValidationException exception = Assert.Throws<TransactionValidationException>(action);

        // Assert
        Assert.Equal("Transaction origin is not supported.", exception.Message);
    }

    [Fact]
    public void Create_WithOptionalReferences_ShouldStoreReferences()
    {
        // Arrange
        Guid categoryId = Guid.NewGuid();
        Guid invoiceId = Guid.NewGuid();

        // Act
        Transaction transaction = Transaction.Create(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 17),
            "Purchase",
            10m,
            TransactionType.Expense,
            TransactionOrigin.Imported,
            categoryId,
            invoiceId);

        // Assert
        Assert.Equal(categoryId, transaction.CategoryId);
        Assert.Equal(invoiceId, transaction.InvoiceId);
    }

    [Fact]
    public void AssignCategory_WithValidCategoryId_ShouldSetCategory()
    {
        // Arrange
        Transaction transaction = CreateTransaction();
        Guid categoryId = Guid.NewGuid();

        // Act
        transaction.AssignCategory(categoryId);

        // Assert
        Assert.Equal(categoryId, transaction.CategoryId);
    }

    [Fact]
    public void AssignCategory_WithEmptyCategoryId_ShouldThrow()
    {
        // Arrange
        Transaction transaction = CreateTransaction();

        // Act
        Action action = () => transaction.AssignCategory(Guid.Empty);

        // Assert
        TransactionValidationException exception = Assert.Throws<TransactionValidationException>(action);
        Assert.Equal("Transaction category is required.", exception.Message);
    }

    [Fact]
    public void Create_WithSameData_ShouldGenerateSameHash()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        DateOnly date = new(2026, 8, 17);

        // Act
        Transaction first = Transaction.Create(
            accountId,
            date,
            "Purchase",
            10m,
            TransactionType.Expense,
            TransactionOrigin.Imported);
        Transaction second = Transaction.Create(
            accountId,
            date,
            "Purchase",
            10m,
            TransactionType.Expense,
            TransactionOrigin.Imported);

        // Assert
        Assert.Equal(first.Hash, second.Hash);
    }

    [Fact]
    public void Create_WithDifferentAccount_ShouldGenerateDifferentHash()
    {
        // Arrange
        DateOnly date = new(2026, 8, 17);

        // Act
        Transaction first = Transaction.Create(
            Guid.NewGuid(),
            date,
            "Purchase",
            10m,
            TransactionType.Expense,
            TransactionOrigin.Imported);
        Transaction second = Transaction.Create(
            Guid.NewGuid(),
            date,
            "Purchase",
            10m,
            TransactionType.Expense,
            TransactionOrigin.Imported);

        // Assert
        Assert.NotEqual(first.Hash, second.Hash);
    }

    #endregion

    #region Test Helpers

    private static Transaction CreateTransaction()
    {
        return Transaction.Create(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 17),
            "Purchase",
            10m,
            TransactionType.Expense,
            TransactionOrigin.Manual);
    }

    #endregion
}
