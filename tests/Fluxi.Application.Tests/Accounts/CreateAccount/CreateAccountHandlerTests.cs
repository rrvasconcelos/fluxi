using Fluxi.Application.Abstractions.Data;
using Fluxi.Domain.Accounts.Entities;
using Fluxi.Domain.Accounts.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fluxi.Application.Tests.Accounts.CreateAccount;

[Trait(TestTraits.Category, TestTraits.UnitCategory)]
[Trait(TestTraits.Layer, TestTraits.ApplicationLayer)]
[Trait(TestTraits.Feature, TestTraits.AccountsFeature)]
public class CreateAccountHandlerTests
{
    #region Fields

    private readonly Mock<IApplicationDbContext> _dbContext;
    private readonly Mock<DbSet<Account>> _accounts;
    private readonly CreateAccountHandler _handler;

    #endregion
    
    #region Constructors

    public CreateAccountHandlerTests()
    {
        _dbContext = new Mock<IApplicationDbContext>();
        _accounts = new Mock<DbSet<Account>>();

        _dbContext
            .SetupGet(context => context.Accounts)
            .Returns(_accounts.Object);

        _handler = new CreateAccountHandler(_dbContext.Object);
    }

    #endregion
    
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

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _accounts.Verify(
            accounts => accounts.Add(It.Is<Account>(account =>
                account.Name == command.Name &&
                account.Bank == command.Bank &&
                account.Type == command.Type &&
                account.ImportMethod == command.ImportMethod &&
                account.Status == AccountStatus.Active)),
            Times.Once);

        _dbContext.Verify(
            context => context.SaveChangesAsync(CancellationToken.None),
            Times.Once);
    }

    #endregion
}