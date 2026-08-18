using Fluxi.Domain.Exceptions;

namespace Fluxi.Domain.Categories;

public sealed class CategoryRuleValidationException : DomainException
{
    #region Constructors

    public CategoryRuleValidationException(string message)
        : base(message)
    {
    }

    #endregion
}
