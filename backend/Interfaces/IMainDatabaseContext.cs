using Microsoft.EntityFrameworkCore;
using Vega.Models;

namespace Vega.Interfaces;

public interface IMainDatabaseContext
{
    DbSet<User> Users { get; set; }
    DbSet<Blog> Blogs { get; set; }
    DbSet<ExpenseGroup> Expense_groups { get; set; }
    DbSet<Expense> Expenses { get; set; }
    DbSet<IncomeGroup> Income_groups { get; set; }
    DbSet<Income> Incomes { get; set; }
    DbSet<Reminder> Reminders { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}