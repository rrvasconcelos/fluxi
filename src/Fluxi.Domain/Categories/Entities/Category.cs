using Fluxi.Domain.Categories.Exceptions;
using Fluxi.Domain.Transactions.Entities;
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

    private readonly List<CategoryRule> _rules = [];

    private readonly List<Transaction> _transactions = [];

    public string Name { get; private set; }

    public IReadOnlyCollection<CategoryRule> Rules => _rules;

    public IReadOnlyCollection<Transaction> Transactions => _transactions;

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
