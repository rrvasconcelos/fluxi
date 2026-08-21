using Fluxi.SharedKernel.Entities;

namespace Fluxi.SharedKernel.Tests.Entities;

[Trait(TestTraits.Category, TestTraits.UnitCategory)]
[Trait(TestTraits.Layer, TestTraits.SharedKernelLayer)]
[Trait(TestTraits.Feature, TestTraits.EntitiesFeature)]
public sealed class AuditableEntityTests
{
    #region Tests

    [Fact]
    public void Create_ShouldSetCreatedOn()
    {
        // Arrange
        DateTime before = DateTime.UtcNow;

        // Act
        TestAuditableEntity entity = new(Guid.NewGuid());

        // Assert
        Assert.InRange(entity.CreatedOn, before, DateTime.UtcNow);
    }

    [Fact]
    public void Create_ShouldLeaveUpdatedOnNull()
    {
        // Arrange & Act
        TestAuditableEntity entity = new(Guid.NewGuid());

        // Assert
        Assert.Null(entity.UpdatedOn);
    }

    [Fact]
    public void Touch_ShouldSetUpdatedOn()
    {
        // Arrange
        TestAuditableEntity entity = new(Guid.NewGuid());
        DateTime before = DateTime.UtcNow;

        // Act
        entity.MarkAsTouched();

        // Assert
        Assert.NotNull(entity.UpdatedOn);
        Assert.InRange(entity.UpdatedOn!.Value, before, DateTime.UtcNow);
    }

    #endregion

    #region Test Doubles

    private sealed class TestAuditableEntity : AuditableEntity<Guid>
    {
        public TestAuditableEntity(Guid id)
            : base(id)
        {
        }

        public void MarkAsTouched()
        {
            Touch();
        }
    }

    #endregion
}
