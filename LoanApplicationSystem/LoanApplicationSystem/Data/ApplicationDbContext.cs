using LoanApplicationSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoanApplicationSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LoanApplication> LoanApplications { get; set; }
        public DbSet<DecisionHistory> DecisionHistories { get; set; }
        public DbSet<CmsPage> CmsPages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<LoanApplication>()
                .Property(x => x.MonthlyIncome)
                .HasPrecision(18, 2);

            builder.Entity<LoanApplication>()
                .Property(x => x.MonthlyExpenses)
                .HasPrecision(18, 2);

            builder.Entity<LoanApplication>()
                .Property(x => x.RequestedAmount)
                .HasPrecision(18, 2);

            builder.Entity<LoanApplication>()
                .Property(x => x.DisposableIncome)
                .HasPrecision(18, 2);

            builder.Entity<LoanApplication>()
                .Property(x => x.EstimatedMonthlyInstallment)
                .HasPrecision(18, 2);

            builder.Entity<LoanApplication>()
                .Property(x => x.DtiPercent)
                .HasPrecision(18, 2);
        }
    }
}