using System.Security.Cryptography;
using System.Text;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class MainDatabaseContext : DbContext, IMainDatabaseContext
{
    public MainDatabaseContext(DbContextOptions<MainDatabaseContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<ExpenseGroup> ExpenseGroups { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<IncomeGroup> IncomeGroups { get; set; }
    public DbSet<Income> Incomes { get; set; }
    public DbSet<Reminder> Reminders { get; set; }

    public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // seed blogs
        // for (int i = 1; i <= 100; i++)
        // {
        // 	modelBuilder.Entity<Blog>().HasData(new Blog
        // 	{
        // 		Id = i,
        // 		Description = $"Blog Description {i}",
        // 		Author = $"http://author{i}.com",
        // 		Text = $"Blog Text {i}"
        // 	});
        // }

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "Administrator",
            Email = "admin@gmail.com",
            Password = HashPassword("password"),
            AccountType = "Administrator"
        });

        modelBuilder.Entity<User>()
            .HasMany(e => e.Blogs)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .IsRequired();

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.User)
            .WithMany(u => u.Expenses)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.ExpenseGroup)
            .WithMany(g => g.Expenses)
            .HasForeignKey(e => e.ExpenseGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Income>()
            .HasOne(e => e.User)
            .WithMany(u => u.Incomes)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Income>()
            .HasOne(i => i.IncomeGroup)
            .WithMany(g => g.Incomes)
            .HasForeignKey(i => i.IncomeGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}