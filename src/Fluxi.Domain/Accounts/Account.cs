using Fluxi.SharedKernel.Entities;

namespace Fluxi.Domain.Accounts;

public sealed class Account : Entity
{
    #region Constructors

    private Account(
        Guid id,
        string name,
        string bank,
        AccountType type,
        ImportMethod importMethod)
        : base(id)
    {
        Name = name;
        Bank = bank;
        Type = type;
        ImportMethod = importMethod;
        Status = AccountStatus.Active;
    }

    #endregion

    #region Properties

    public string Name { get; private set; }

    public string Bank { get; private set; }

    public AccountType Type { get; }

    public ImportMethod ImportMethod { get; private set; }

    public AccountStatus Status { get; private set; }

    #endregion

    #region Factory Methods

    public static Account Create(
        string name,
        string bank,
        AccountType type,
        ImportMethod importMethod)
    {
        Validate(name, bank, type, importMethod);

        return new Account(Guid.NewGuid(), name, bank, type, importMethod);
    }

    #endregion

    #region Methods

    public void ChangeName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    public void ChangeBank(string bank)
    {
        ValidateBank(bank);
        Bank = bank;
    }

    public void ChangeImportMethod(ImportMethod importMethod)
    {
        ValidateImportMethod(importMethod);
        ImportMethod = importMethod;
    }

    public void Deactivate()
    {
        Status = AccountStatus.Inactive;
    }

    public void Activate()
    {
        Status = AccountStatus.Active;
    }

    #endregion

    #region Validation

    private static void Validate(
        string name,
        string bank,
        AccountType type,
        ImportMethod importMethod)
    {
        ValidateName(name);

        ValidateBank(bank);

        if (!Enum.IsDefined(type))
        {
            throw new AccountValidationException("Account type is not supported.");
        }

        ValidateImportMethod(importMethod);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new AccountValidationException("Account name is required.");
        }
    }

    private static void ValidateBank(string bank)
    {
        if (string.IsNullOrWhiteSpace(bank))
        {
            throw new AccountValidationException("Account bank is required.");
        }
    }

    private static void ValidateImportMethod(ImportMethod importMethod)
    {
        if (!Enum.IsDefined(importMethod))
        {
            throw new AccountValidationException("Import method is not supported.");
        }
    }

    #endregion
}