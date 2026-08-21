using Fluxi.Domain.Incomes.Entities;
using Fluxi.Domain.Incomes.Exceptions;
using Fluxi.Domain.Accounts.Entities;
using Fluxi.Domain.Accounts.Enums;

namespace Fluxi.Domain.Tests.Incomes;

[Trait(TestTraits.Category, TestTraits.UnitCategory)]
[Trait(TestTraits.Layer, TestTraits.DomainLayer)]
[Trait(TestTraits.Feature, TestTraits.IncomesFeature)]
public sealed class IncomeTests
{
    #region Tests

    [Fact]
    public void Create_WithValidRecurringIncome_ShouldCreateIncome()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        const string description = "Monthly salary";
        const decimal amount = 5000m;
        const int receiptDay = 5;

        // Act
        Income income = Income.Create(accountId, description, amount, receiptDay);

        // Assert
        Assert.NotEqual(Guid.Empty, income.Id);
        Assert.Equal(accountId, income.AccountId);
        Assert.Equal(description, income.Description);
        Assert.Equal(amount, income.Amount);
        Assert.Equal(receiptDay, income.ReceiptDay);
        Assert.True(income.IsRecurring);
    }

    [Fact]
    public void Create_ShouldExposeAccountNavigation()
    {
        // Arrange
        Account account = Account.Create(
            "Nubank",
            "Nubank",
            AccountType.Checking,
            ImportMethod.Ofx);

        // Act
        Income income = Income.Create(account.Id, "Monthly salary", 5000m, 5);

        // Assert
        Assert.Null(income.Account);
    }

    [Fact]
    public void Create_WithNonRecurringIncome_ShouldCreateIncome()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();

        // Act
        Income income = Income.Create(accountId, "Annual bonus", 1000m, 15, false);

        // Assert
        Assert.False(income.IsRecurring);
    }

    [Fact]
    public void Create_WithEmptyAccountId_ShouldThrow()
    {
        // Arrange
        Action action = () => Income.Create(Guid.Empty, "Monthly salary", 5000m, 5);

        // Act
        IncomeValidationException exception = Assert.Throws<IncomeValidationException>(action);

        // Assert
        Assert.Equal("Income account is required.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidDescription_ShouldThrow(string? description)
    {
        // Arrange
        Action action = () => Income.Create(Guid.NewGuid(), description!, 5000m, 5);

        // Act
        IncomeValidationException exception = Assert.Throws<IncomeValidationException>(action);

        // Assert
        Assert.Equal("Income description is required.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void Create_WithNonPositiveAmount_ShouldThrow(decimal amount)
    {
        // Arrange
        Action action = () => Income.Create(Guid.NewGuid(), "Monthly salary", amount, 5);

        // Act
        IncomeValidationException exception = Assert.Throws<IncomeValidationException>(action);

        // Assert
        Assert.Equal("Income amount must be greater than zero.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void Create_WithReceiptDayOutsideMonthRange_ShouldThrow(int receiptDay)
    {
        // Arrange
        Action action = () => Income.Create(Guid.NewGuid(), "Monthly salary", 5000m, receiptDay);

        // Act
        IncomeValidationException exception = Assert.Throws<IncomeValidationException>(action);

        // Assert
        Assert.Equal("Income receipt day must be between 1 and 31.", exception.Message);
    }

    #endregion
}
