namespace Fluxi.SharedKernel.Entities;

public abstract class AuditableEntity : Entity
{
    #region Constructors

    protected AuditableEntity(Guid id)
        : base(id)
    {
        CreatedOn = DateTime.UtcNow;
    }

    #endregion

    #region Properties

    public DateTime CreatedOn { get; private set; }

    public DateTime? UpdatedOn { get; private set; }

    #endregion

    #region Methods

    protected void Touch()
    {
        UpdatedOn = DateTime.UtcNow;
    }

    #endregion
}