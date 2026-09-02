using Fluxi.Application.Features.Accounts.CreateAccount;
using Fluxi.Domain.Accounts.Entities;
using Fluxi.Domain.Accounts.Enums;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Fluxi.Application.Tests.Accounts.CreateAccount;

[Trait(TestTraits.Category, TestTraits.IntegrationCategory)]
[Trait(TestTraits.Layer, TestTraits.ApplicationLayer)]
[Trait(TestTraits.Feature, TestTraits.AccountsFeature)]
public class CreateAccountHandlerTests
{
    #region Tests

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistActiveAccount()
    {
        // Arrange
        var command = new CreateAccountCommand(
            "Nubank Credit Card",
            "Nubank",
            AccountType.CreditCard,
            ImportMethod.Ofx);

        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var context = CreateDbContext(connection);
        await context.Database.EnsureCreatedAsync();

        var handler = new CreateAccountCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        context.ChangeTracker.Clear();

        var account = await context.Accounts.SingleAsync();
        Assert.Equal(command.Name, account.Name);
        Assert.Equal(command.Bank, account.Bank);
        Assert.Equal(command.Type, account.Type);
        Assert.Equal(command.ImportMethod, account.ImportMethod);
        Assert.Equal(AccountStatus.Active, account.Status);
    }

    [Fact]
    public async Task Handle_WithExistingAccount_ShouldReturnError()
    {
        // Arrange
        var command = new CreateAccountCommand(
            "Nubank Credit Card",
            "Nubank",
            AccountType.CreditCard,
            ImportMethod.Ofx);

        var existingAccount = Account.Create(
            command.Name,
            command.Bank,
            command.Type,
            command.ImportMethod);

        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var context = CreateDbContext(connection);
        await context.Database.EnsureCreatedAsync();

        context.Accounts.Add(existingAccount);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var handler = new CreateAccountCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("An account with the same name already exists.", result.Error.Description);
        Assert.Equal(1, await context.Accounts.CountAsync());
    }

    #endregion

    #region Helpers

    private static CreateAccountDbContext CreateDbContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<CreateAccountDbContext>()
            .UseSqlite(connection)
            .Options;

        return new CreateAccountDbContext(options);
    }

    #endregion
}
