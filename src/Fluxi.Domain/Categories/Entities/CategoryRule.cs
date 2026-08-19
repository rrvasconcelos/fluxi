using Fluxi.Domain.Categories.Exceptions;
using Fluxi.SharedKernel.Entities;

namespace Fluxi.Domain.Categories.Entities;

public sealed class CategoryRule : AuditableEntity
{
    #region Constructors

    private CategoryRule(Guid id, string pattern, Category category, int priority)
        : base(id)
    {
        Pattern = pattern;
        Category = category;
        Priority = priority;
    }

    #endregion

    #region Properties

    public string Pattern { get; }

    public Category Category { get; }

    public int Priority { get; }

    #endregion

    #region Factory Methods

    public static CategoryRule Create(string pattern, Category category, int priority)
    {
        Validate(pattern, category, priority);

        return new CategoryRule(Guid.NewGuid(), pattern.Trim(), category, priority);
    }

    #endregion

    #region Methods

    public bool Matches(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return false;
        }

        return description.Contains(Pattern, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Validation

    private static void Validate(string pattern, Category category, int priority)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            throw new CategoryRuleValidationException("Category rule pattern is required.");
        }

        if (category is null)
        {
            throw new CategoryRuleValidationException("Category is required.");
        }

        if (priority < 0)
        {
            throw new CategoryRuleValidationException("Category rule priority cannot be negative.");
        }
    }

    #endregion
}
