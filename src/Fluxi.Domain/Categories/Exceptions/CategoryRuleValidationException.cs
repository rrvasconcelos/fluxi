using Fluxi.Domain.Exceptions;

namespace Fluxi.Domain.Categories.Exceptions;

public sealed class CategoryRuleValidationException : DomainException
{
    #region Constructors

    public CategoryRuleValidationException(string message)
        : base(message)
    {
    }

    #endregion
}
