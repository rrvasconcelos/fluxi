namespace Fluxi.SharedKernel.Entities;

public abstract class Entity<TId>
    where TId : notnull
{
    #region Constructors

    protected Entity(TId id)
    {
        if (EqualityComparer<TId>.Default.Equals(id, default!))
        {
            throw new ArgumentException("Entity identity cannot be empty.", nameof(id));
        }

        Id = id;
    }

    #endregion

    #region Properties

    public TId Id { get; }

    #endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity
            && entity.GetType() == GetType()
            && EqualityComparer<TId>.Default.Equals(entity.Id, Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }

    #endregion
}
