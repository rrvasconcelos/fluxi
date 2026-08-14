using Fluxi.SharedKernel.Entities;

namespace Fluxi.SharedKernel.Tests.Entities;

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

    #endregion

    #region Test Doubles

    private sealed class TestEntity : Entity
    {
        public TestEntity(Guid id)
            : base(id)
        {
        }
    }

    #endregion
}