using Fluxi.Domain.Categories.Entities;
using Fluxi.Domain.Categories.Exceptions;

namespace Fluxi.Domain.Tests.Categories;

[Trait(TestTraits.Category, TestTraits.UnitCategory)]
[Trait(TestTraits.Layer, TestTraits.DomainLayer)]
[Trait(TestTraits.Feature, TestTraits.CategoriesFeature)]
public sealed class CategoryTests
{
    #region Tests

    [Fact]
    public void Create_WithValidName_ShouldCreateCategory()
    {
        // Arrange
        const string name = "Food";

        // Act
        Category category = Category.Create(name);

        // Assert
        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.Equal(name, category.Name);
    }

    [Fact]
    public void Create_ShouldInitializeRelatedEntities()
    {
        // Arrange

        // Act
        Category category = Category.Create("Food");

        // Assert
        Assert.Empty(category.Rules);
        Assert.Empty(category.Transactions);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidName_ShouldThrow(string? name)
    {
        // Arrange
        Action action = () => Category.Create(name!);

        // Act
        CategoryValidationException exception = Assert.Throws<CategoryValidationException>(action);

        // Assert
        Assert.Equal("Category name is required.", exception.Message);
    }

    [Fact]
    public void ChangeName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        Category category = Category.Create("Food");

        // Act
        category.ChangeName("Groceries");

        // Assert
        Assert.Equal("Groceries", category.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ChangeName_WithInvalidName_ShouldThrow(string? name)
    {
        // Arrange
        Category category = Category.Create("Food");

        // Act
        Action action = () => category.ChangeName(name!);

        // Assert
        CategoryValidationException exception = Assert.Throws<CategoryValidationException>(action);
        Assert.Equal("Category name is required.", exception.Message);
    }

    #endregion
}
