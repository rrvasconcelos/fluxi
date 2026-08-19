using Fluxi.Domain.Exceptions;

namespace Fluxi.Domain.Categories.Exceptions;

public sealed class CategoryValidationException : DomainException
{
    #region Constructors

    public CategoryValidationException(string message)
        : base(message)
    {
    }

    #endregion
}
