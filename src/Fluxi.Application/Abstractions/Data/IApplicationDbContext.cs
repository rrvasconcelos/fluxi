using Fluxi.Domain.Accounts.Entities;
using Fluxi.Domain.Categories.Entities;
using Fluxi.Domain.FixedExpenses.Entities;
using Fluxi.Domain.Incomes.Entities;
using Fluxi.Domain.Invoices.Entities;
using Fluxi.Domain.Transactions.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fluxi.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }
    DbSet<Category> Categories { get; }
    DbSet<FixedExpense> FixedExpenses { get; }
    DbSet<Income> Incomes { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<Transaction> Transactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}