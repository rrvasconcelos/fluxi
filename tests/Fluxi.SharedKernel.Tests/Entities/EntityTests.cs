using Fluxi.SharedKernel.Entities;

namespace Fluxi.SharedKernel.Tests.Entities;

[Trait(TestTraits.Category, TestTraits.UnitCategory)]
[Trait(TestTraits.Layer, TestTraits.SharedKernelLayer)]
[Trait(TestTraits.Feature, TestTraits.EntitiesFeature)]
public sealed class EntityTests
{
    #region Tests

    [Fact]
    public void Create_WithEmptyId_ShouldThrow()
    {
        // Arrange
        Action action = () => new TestEntity(Guid.Empty);

        // Act
        ArgumentException exception = Assert.Throws<ArgumentException>(action);

        // Assert
        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void Equals_WithSameIdAndType_ShouldReturnTrue()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        TestEntity first = new(id);
        TestEntity second = new(id);

        // Act
        bool result = first.Equals(second);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_WithDifferentIds_ShouldReturnFalse()
    {
        // Arrange
        TestEntity first = new(Guid.NewGuid());
        TestEntity second = new(Guid.NewGuid());

        // Act
        bool result = first.Equals(second);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetHashCode_WithSameIdAndType_ShouldReturnSameValue()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        TestEntity first = new(id);
        TestEntity second = new(id);

        // Act
        int firstHashCode = first.GetHashCode();
        int secondHashCode = second.GetHashCode();

        // Assert
        Assert.Equal(firstHashCode, secondHashCode);
    }

    [Fact]
    public void Create_WithDefaultNonGuidId_ShouldThrow()
    {
        // Arrange
        Action action = () => new TestIntEntity(0);

        // Act
        ArgumentException exception = Assert.Throws<ArgumentException>(action);

        // Assert
        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void Equals_WithNonGuidIdAndSameValue_ShouldReturnTrue()
    {
        // Arrange
        TestIntEntity first = new(1);
        TestIntEntity second = new(1);

        // Act
        bool result = first.Equals(second);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_WithNonGuidIdAndDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        TestIntEntity first = new(1);
        TestIntEntity second = new(2);

        // Act
        bool result = first.Equals(second);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Test Doubles

    private sealed class TestEntity : Entity<Guid>
    {
        public TestEntity(Guid id)
            : base(id)
        {
        }
    }

    private sealed class TestIntEntity : Entity<int>
    {
        public TestIntEntity(int id)
            : base(id)
        {
        }
    }

    #endregion
}