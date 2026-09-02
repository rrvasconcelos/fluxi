using FluentValidation;

namespace Fluxi.Application.Features.Accounts.CreateAccount;

public class CreateAccountCommandValidator: AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required.");
        
        RuleFor(command => command.Bank)
            .NotEmpty().WithMessage("Bank is required.");
        
        RuleFor(command => command.Type)
            .IsInEnum().WithMessage("Type is required.");
        
        RuleFor(command => command.ImportMethod)
            .IsInEnum().WithMessage("Import method is required.");
    }
}

