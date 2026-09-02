using Fluxi.Application.Abstractions.Data;
using Fluxi.Domain.Accounts.Entities;
using Fluxi.Domain.Categories.Entities;
using Fluxi.Domain.FixedExpenses.Entities;
using Fluxi.Domain.Incomes.Entities;
using Fluxi.Domain.Invoices.Entities;
using Fluxi.Domain.Transactions.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fluxi.Application.Tests.Accounts.CreateAccount;

internal sealed class CreateAccountDbContext(DbContextOptions<CreateAccountDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    #region Properties

    public DbSet<Account> Accounts => Set<Account>();

    DbSet<Category> IApplicationDbContext.Categories => throw new NotSupportedException();

    DbSet<FixedExpense> IApplicationDbContext.FixedExpenses => throw new NotSupportedException();

    DbSet<Income> IApplicationDbContext.Incomes => throw new NotSupportedException();

    DbSet<Invoice> IApplicationDbContext.Invoices => throw new NotSupportedException();

    DbSet<Transaction> IApplicationDbContext.Transactions => throw new NotSupportedException();

    #endregion

    #region Methods

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Category>();
        modelBuilder.Ignore<FixedExpense>();
        modelBuilder.Ignore<Income>();
        modelBuilder.Ignore<Invoice>();
        modelBuilder.Ignore<Transaction>();

        modelBuilder.Entity<Account>(builder =>
        {
            builder.ToTable("account");
            builder.HasKey(account => account.Id);
            builder.Property(account => account.Id).ValueGeneratedNever();
            builder.Property(account => account.Name).IsRequired();
            builder.Property(account => account.Bank).IsRequired();
            builder.Property(account => account.Type).HasConversion<string>();
            builder.Property(account => account.ImportMethod).HasConversion<string>();
            builder.Property(account => account.Status).HasConversion<string>();
            builder.HasIndex(account => account.Name).IsUnique();
            builder.Ignore(account => account.Incomes);
            builder.Ignore(account => account.Invoices);
            builder.Ignore(account => account.Transactions);
        });
    }

    #endregion
}
