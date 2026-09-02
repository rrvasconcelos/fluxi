using Fluxi.Application;
using Fluxi.Application.Abstractions.Data;
using Fluxi.Application.Abstractions.Messaging;
using Fluxi.Application.Features.Accounts.CreateAccount;
using Fluxi.Domain.Accounts.Enums;
using Fluxi.SharedKernel.Results;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Fluxi.Application.Tests.Accounts.CreateAccount;

[Trait(TestTraits.Category, TestTraits.IntegrationCategory)]
[Trait(TestTraits.Layer, TestTraits.ApplicationLayer)]
[Trait(TestTraits.Feature, TestTraits.AccountsFeature)]
public class CreateAccountCommandValidationPipelineTests
{
    #region Tests

    [Fact]
    public async Task Handle_WithInvalidCommand_ShouldReturnValidationErrorWithoutInvokingInnerHandler()
    {
        // Arrange
        var context = new Mock<IApplicationDbContext>();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<IApplicationDbContext>(_ => context.Object);
        services.AddApplication();

        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();

        var handler = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<CreateAccountCommand>>();

        var command = new CreateAccountCommand(
            string.Empty,
            string.Empty,
            (AccountType)999,
            (ImportMethod)999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<ValidationError>(result.Error);
        context.VerifyGet(dbContext => dbContext.Accounts, Times.Never);
    }

    #endregion
}