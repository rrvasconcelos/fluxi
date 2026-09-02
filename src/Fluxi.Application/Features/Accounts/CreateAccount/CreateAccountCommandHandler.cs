using Fluxi.Application.Abstractions.Data;
using Fluxi.Application.Abstractions.Messaging;
using Fluxi.Domain.Accounts.Entities;
using Fluxi.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Fluxi.Application.Features.Accounts.CreateAccount;

public sealed class CreateAccountCommandHandler(IApplicationDbContext context) : ICommandHandler<CreateAccountCommand>
{
    public async Task<Result> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var accountNameExists = await context
            .Accounts
            .AsNoTracking()
            .AnyAsync(account => account.Name == command.Name, cancellationToken);

        if (accountNameExists)
        {
            return Result.Failure(Error.Conflict(
                "Account.Duplicate",
                "An account with the same name already exists."));
        }

        var account = Account.Create(command.Name, command.Bank, command.Type, command.ImportMethod);

        context.Accounts.Add(account);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

