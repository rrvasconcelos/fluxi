using FluentValidation.TestHelper;
using Fluxi.Application.Features.Accounts.CreateAccount;
using Fluxi.Domain.Accounts.Enums;

namespace Fluxi.Application.Tests.Accounts.CreateAccount;

[Trait(TestTraits.Category, TestTraits.UnitCategory)]
[Trait(TestTraits.Layer, TestTraits.ApplicationLayer)]
[Trait(TestTraits.Feature, TestTraits.AccountsFeature)]
public class CreateAccountCommandValidatorTests
{
    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        // Arrange
        var command = new CreateAccountCommand(
            string.Empty,
            string.Empty,
            AccountType.Checking,
            ImportMethod.Manual
        );
        var validator = new CreateAccountCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Bank_Is_Empty()
    {
        // Arrange
        var command = new CreateAccountCommand(
            "Test Name",
            string.Empty,
            AccountType.Checking,
            ImportMethod.Manual
        );
        var validator = new CreateAccountCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Bank);
    }

    [Fact]
    public void Should_Have_Error_When_Type_Is_Invalid()
    {
        // Arrange
        var command = new CreateAccountCommand(
            "Test Name",
            "Test Bank",
            (AccountType)999,
            ImportMethod.Manual
        );
        var validator = new CreateAccountCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Type);
    }

    [Fact]
    public void Should_Have_Error_When_ImportMethod_Is_Invalid()
    {
        // Arrange
        var command = new CreateAccountCommand(
            "Test Name",
            "Test Bank",
            AccountType.Checking,
            (ImportMethod)999
        );
        var validator = new CreateAccountCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.ImportMethod);
    }
}