using Fluxi.Application.Abstractions.Messaging;
using Fluxi.Domain.Accounts.Enums;

namespace Fluxi.Application.Features.Accounts.CreateAccount;

public sealed record CreateAccountCommand(
    string Name, 
    string Bank, 
    AccountType Type, 
    ImportMethod ImportMethod) : ICommand;

