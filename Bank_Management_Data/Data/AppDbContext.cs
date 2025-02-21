using Bank_Management_Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Management_Data.Data
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<FundTransaction> Transactions { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Account>()
                .HasOne(a=>a.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Account>()
                .HasIndex(a => a.AccountNumber)
                .IsUnique();
            builder.Entity<Account>()
                .HasOne(cu => cu.Currency)
                .WithMany(a => a.Accounts)
                .HasForeignKey(cu => cu.CurrencyId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<FundTransaction>()
                .HasKey(a => a.TransactionId);
            builder.Entity<AccountType>()
                .HasKey(k=>k.Id);
            builder.Entity<Account>()
                .HasOne(a=>a.AccountType)
                .WithMany(at=>at.Accounts)
                .HasForeignKey(a=>a.AccountTypeId)
                .OnDelete(DeleteBehavior.Cascade);
                
        }
    }
}
