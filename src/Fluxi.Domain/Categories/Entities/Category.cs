using Fluxi.Domain.Categories.Exceptions;
using Fluxi.SharedKernel.Entities;

namespace Fluxi.Domain.Categories.Entities;

public sealed class Category : AuditableEntity<Guid>
{
    #region Constructors

    private Category(Guid id, string name)
        : base(id)
    {
        Name = name;
    }

    #endregion

    #region Properties

    public string Name { get; private set; }

    #endregion

    #region Factory Methods

    public static Category Create(string name)
    {
        ValidateName(name);

        return new Category(Guid.NewGuid(), name.Trim());
    }

    #endregion

    #region Methods

    public void ChangeName(string name)
    {
        ValidateName(name);

        Name = name.Trim();
    }

    #endregion

    #region Validation

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new CategoryValidationException("Category name is required.");
        }
    }

    #endregion
}
