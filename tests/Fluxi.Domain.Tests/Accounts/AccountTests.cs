using Fluxi.Domain.Accounts.Entities;
using Fluxi.Domain.Accounts.Enums;
using Fluxi.Domain.Accounts.Exceptions;

namespace Fluxi.Domain.Tests.Accounts;

[Trait(TestTraits.Category, TestTraits.UnitCategory)]
[Trait(TestTraits.Layer, TestTraits.DomainLayer)]
[Trait(TestTraits.Feature, TestTraits.AccountsFeature)]
public sealed class AccountTests
{
    #region Tests

    [Fact]
    public void Create_WithValidData_ShouldCreateAccount()
    {
        // Arrange
        const string name = "Nubank Credit Card";
        const string bank = "Nubank";
        const AccountType type = AccountType.CreditCard;
        const ImportMethod importMethod = ImportMethod.Ofx;

        // Act
        Account account = Account.Create(
            name,
            bank,
            type,
            importMethod);

        // Assert
        Assert.NotEqual(Guid.Empty, account.Id);
        Assert.Equal(AccountStatus.Active, account.Status);
        Assert.Equal(name, account.Name);
        Assert.Equal(bank, account.Bank);
        Assert.Equal(type, account.Type);
        Assert.Equal(importMethod, account.ImportMethod);
    }

    [Fact]
    public void Create_ShouldInitializeRelatedEntities()
    {
        // Arrange

        // Act
        Account account = CreateAccount();

        // Assert
        Assert.Empty(account.Incomes);
        Assert.Empty(account.Invoices);
        Assert.Empty(account.Transactions);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidName_ShouldThrow(string? name)
    {
        // Arrange
        Action action = () => Account.Create(
            name!,
            "Nubank",
            AccountType.CreditCard,
            ImportMethod.Ofx);

        // Act
        AccountValidationException exception = Assert.Throws<AccountValidationException>(action);

        // Assert
        Assert.Equal("Account name is required.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidBank_ShouldThrow(string? bank)
    {
        // Arrange
        Action action = () => Account.Create(
            "Nubank Credit Card",
            bank!,
            AccountType.CreditCard,
            ImportMethod.Ofx);

        // Act
        AccountValidationException exception = Assert.Throws<AccountValidationException>(action);

        // Assert
        Assert.Equal("Account bank is required.", exception.Message);
    }

    [Fact]
    public void Create_WithUnsupportedAccountType_ShouldThrow()
    {
        // Arrange
        Action action = () => Account.Create(
            "Nubank Credit Card",
            "Nubank",
            (AccountType)999,
            ImportMethod.Ofx);

        // Act
        AccountValidationException exception = Assert.Throws<AccountValidationException>(action);

        // Assert
        Assert.Equal("Account type is not supported.", exception.Message);
    }

    [Fact]
    public void Create_WithUnsupportedImportMethod_ShouldThrow()
    {
        // Arrange
        Action action = () => Account.Create(
            "Nubank Credit Card",
            "Nubank",
            AccountType.CreditCard,
            (ImportMethod)999);

        // Act
        AccountValidationException exception = Assert.Throws<AccountValidationException>(action);

        // Assert
        Assert.Equal("Import method is not supported.", exception.Message);
    }

    [Fact]
    public void ChangeName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        Account account = CreateAccount();

        // Act
        account.ChangeName("Updated Account");

        // Assert
        Assert.Equal("Updated Account", account.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ChangeName_WithInvalidName_ShouldThrow(string? name)
    {
        // Arrange
        Account account = CreateAccount();

        // Act
        Action action = () => account.ChangeName(name!);

        // Assert
        AccountValidationException exception = Assert.Throws<AccountValidationException>(action);
        Assert.Equal("Account name is required.", exception.Message);
    }

    [Fact]
    public void ChangeBank_WithValidBank_ShouldUpdateBank()
    {
        // Arrange
        Account account = CreateAccount();

        // Act
        account.ChangeBank("Bradesco");

        // Assert
        Assert.Equal("Bradesco", account.Bank);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ChangeBank_WithInvalidBank_ShouldThrow(string? bank)
    {
        // Arrange
        Account account = CreateAccount();

        // Act
        Action action = () => account.ChangeBank(bank!);

        // Assert
        AccountValidationException exception = Assert.Throws<AccountValidationException>(action);
        Assert.Equal("Account bank is required.", exception.Message);
    }

    [Fact]
    public void ChangeImportMethod_WithSupportedMethod_ShouldUpdateImportMethod()
    {
        // Arrange
        Account account = CreateAccount();

        // Act
        account.ChangeImportMethod(ImportMethod.Csv);

        // Assert
        Assert.Equal(ImportMethod.Csv, account.ImportMethod);
    }

    [Fact]
    public void Deactivate_ActiveAccount_ShouldSetInactive()
    {
        // Arrange
        Account account = CreateAccount();

        // Act
        account.Deactivate();

        // Assert
        Assert.Equal(AccountStatus.Inactive, account.Status);
    }

    [Fact]
    public void Deactivate_InactiveAccount_ShouldKeepInactive()
    {
        // Arrange
        Account account = CreateAccount();
        account.Deactivate();

        // Act
        account.Deactivate();

        // Assert
        Assert.Equal(AccountStatus.Inactive, account.Status);
    }

    [Fact]
    public void Activate_InactiveAccount_ShouldSetActive()
    {
        // Arrange
        Account account = CreateAccount();
        account.Deactivate();

        // Act
        account.Activate();

        // Assert
        Assert.Equal(AccountStatus.Active, account.Status);
    }

    [Fact]
    public void Activate_ActiveAccount_ShouldKeepActive()
    {
        // Arrange
        Account account = CreateAccount();

        // Act
        account.Activate();

        // Assert
        Assert.Equal(AccountStatus.Active, account.Status);
    }

    #region Test Helpers

    private static Account CreateAccount()
    {
        return Account.Create(
            "Nubank Credit Card",
            "Nubank",
            AccountType.CreditCard,
            ImportMethod.Ofx);
    }

    #endregion

    #endregion
}
