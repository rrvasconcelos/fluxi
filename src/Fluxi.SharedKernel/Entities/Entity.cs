namespace Fluxi.SharedKernel.Entities;

public abstract class Entity
{
    #region Constructors

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Entity identity cannot be empty.", nameof(id));
        }

        Id = id;
    }

    #endregion

    #region Properties

    public Guid Id { get; }

    #endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        return obj is Entity entity
            && entity.GetType() == GetType()
            && entity.Id == Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }

    #endregion
}
