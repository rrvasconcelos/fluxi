namespace Fluxi.SharedKernel.Entities;

public abstract class AuditableEntity(Guid id) : Entity(id)
{
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}