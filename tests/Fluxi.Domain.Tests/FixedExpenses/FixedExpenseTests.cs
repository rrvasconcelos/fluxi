using Fluxi.Domain.FixedExpenses.Entities;
using Fluxi.Domain.FixedExpenses.Exceptions;

namespace Fluxi.Domain.Tests.FixedExpenses;

[Trait(TestTraits.Category, TestTraits.UnitCategory)]
[Trait(TestTraits.Layer, TestTraits.DomainLayer)]
[Trait(TestTraits.Feature, TestTraits.FixedExpensesFeature)]
public sealed class FixedExpenseTests
{
    #region Tests

    [Fact]
    public void Create_WithValidData_ShouldCreateFixedExpense()
    {
        // Arrange
        const string description = "Monthly rent";
        const decimal amount = 1500m;
        const int dueDay = 10;
        const string notes = "Pay by bank transfer";

        // Act
        FixedExpense fixedExpense = FixedExpense.Create(description, amount, dueDay, notes);

        // Assert
        Assert.NotEqual(Guid.Empty, fixedExpense.Id);
        Assert.Equal(description, fixedExpense.Description);
        Assert.Equal(amount, fixedExpense.Amount);
        Assert.Equal(dueDay, fixedExpense.DueDay);
        Assert.Equal(notes, fixedExpense.Notes);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidDescription_ShouldThrow(string? description)
    {
        // Arrange
        Action action = () => FixedExpense.Create(description!, 1500m, 10);

        // Act
        FixedExpenseValidationException exception = Assert.Throws<FixedExpenseValidationException>(action);

        // Assert
        Assert.Equal("Fixed expense description is required.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void Create_WithNonPositiveAmount_ShouldThrow(decimal amount)
    {
        // Arrange
        Action action = () => FixedExpense.Create("Monthly rent", amount, 10);

        // Act
        FixedExpenseValidationException exception = Assert.Throws<FixedExpenseValidationException>(action);

        // Assert
        Assert.Equal("Fixed expense amount must be greater than zero.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void Create_WithDueDayOutsideMonthRange_ShouldThrow(int dueDay)
    {
        // Arrange
        Action action = () => FixedExpense.Create("Monthly rent", 1500m, dueDay);

        // Act
        FixedExpenseValidationException exception = Assert.Throws<FixedExpenseValidationException>(action);

        // Assert
        Assert.Equal("Fixed expense due day must be between 1 and 31.", exception.Message);
    }

    [Fact]
    public void Create_WithPaddedValues_ShouldTrimDescriptionAndNotes()
    {
        // Arrange
        const string description = "  Monthly rent  ";
        const string notes = "  Pay by bank transfer  ";

        // Act
        FixedExpense fixedExpense = FixedExpense.Create(description, 1500m, 10, notes);

        // Assert
        Assert.Equal("Monthly rent", fixedExpense.Description);
        Assert.Equal("Pay by bank transfer", fixedExpense.Notes);
    }

    [Fact]
    public void Create_WithoutNotes_ShouldLeaveNotesNull()
    {
        // Arrange

        // Act
        FixedExpense fixedExpense = FixedExpense.Create("Monthly rent", 1500m, 10);

        // Assert
        Assert.Null(fixedExpense.Notes);
    }

    #endregion
}
