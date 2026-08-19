using Fluxi.Domain.Categories.Entities;
using Fluxi.Domain.Categories.Exceptions;

namespace Fluxi.Domain.Tests.Categories;

public sealed class CategoryRuleTests
{
    #region Tests

    [Fact]
    public void Create_WithValidData_ShouldCreateCategoryRule()
    {
        // Arrange
        Category category = Category.Create("Food");
        const string pattern = "IFOOD";
        const int priority = 10;

        // Act
        CategoryRule categoryRule = CategoryRule.Create(pattern, category, priority);

        // Assert
        Assert.NotEqual(Guid.Empty, categoryRule.Id);
        Assert.Equal(pattern, categoryRule.Pattern);
        Assert.Equal(category, categoryRule.Category);
        Assert.Equal(priority, categoryRule.Priority);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidPattern_ShouldThrow(string? pattern)
    {
        // Arrange
        Category category = Category.Create("Food");

        // Act
        Action action = () => CategoryRule.Create(pattern!, category, 5);

        // Assert
        CategoryRuleValidationException exception = Assert.Throws<CategoryRuleValidationException>(action);
        Assert.Equal("Category rule pattern is required.", exception.Message);
    }

    [Fact]
    public void Create_WithNegativePriority_ShouldThrow()
    {
        // Arrange
        Category category = Category.Create("Food");

        // Act
        Action action = () => CategoryRule.Create("IFOOD", category, -1);

        // Assert
        CategoryRuleValidationException exception = Assert.Throws<CategoryRuleValidationException>(action);
        Assert.Equal("Category rule priority cannot be negative.", exception.Message);
    }

    [Fact]
    public void Matches_WithMatchingPattern_ShouldReturnTrue()
    {
        // Arrange
        Category category = Category.Create("Food");
        CategoryRule categoryRule = CategoryRule.Create("IFOOD", category, 5);

        // Act
        bool matches = categoryRule.Matches("Compra no IFOOD");

        // Assert
        Assert.True(matches);
    }

    [Fact]
    public void Matches_WithNonMatchingPattern_ShouldReturnFalse()
    {
        // Arrange
        Category category = Category.Create("Food");
        CategoryRule categoryRule = CategoryRule.Create("IFOOD", category, 5);

        // Act
        bool matches = categoryRule.Matches("Compra em mercado");

        // Assert
        Assert.False(matches);
    }

    [Fact]
    public void Matches_WithDifferentCase_ShouldReturnTrue()
    {
        // Arrange
        Category category = Category.Create("Food");
        CategoryRule categoryRule = CategoryRule.Create("IFOOD", category, 5);

        // Act
        bool matches = categoryRule.Matches("ifood mercado");

        // Assert
        Assert.True(matches);
    }

    #endregion
}
