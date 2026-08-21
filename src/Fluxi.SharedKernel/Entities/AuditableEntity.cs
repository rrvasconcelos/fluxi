namespace Fluxi.SharedKernel.Entities;

public abstract class AuditableEntity<TId>(TId id) : Entity<TId>(id)
    where TId : notnull
{
    #region Properties

    public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedOn { get; private set; }

    #endregion

    #region Methods

    protected void Touch()
    {
        UpdatedOn = DateTime.UtcNow;
    }

    #endregion
}