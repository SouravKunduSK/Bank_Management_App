using Bank_Management_Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Management_Data.Data
{
    public class DbInitializer
    {
        public static async Task SeedDefaultAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateAsyncScope()) 
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    //Check Database is created
                    await context.Database.EnsureCreatedAsync();

                    //seed account type
                    if (context.AccountTypes.Any())
                    {
                        return;
                    }

                    var newAccountTypes = new AccountType[] {
                        new AccountType { TypeName = "Savings", DailyMoneyTransactionLimit = 100000,DailyTransactionNumberLimit = 10, TransactionFee = 10 },
                        new AccountType {  TypeName = "Current", DailyMoneyTransactionLimit = 200000,DailyTransactionNumberLimit = 15, TransactionFee = 5 },
                        new AccountType {  TypeName = "Business", DailyMoneyTransactionLimit = 500000,DailyTransactionNumberLimit = 20, TransactionFee = 2 }
                         };
                    await context.AccountTypes.AddRangeAsync(newAccountTypes);
                    await context.SaveChangesAsync();


                    if (context.UserRoles.Any() || context.Users.Any())
                    {
                        return;
                    }

                    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                    //seed roles
                    var roles = new[] { "Admin", "BankStaff", "Customer" };
                    foreach (var role in roles) 
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }

                    //seed admin
                    var adminEmail = "180144.cse@student.just.edu.bd";
                    if(await userManager.FindByEmailAsync(adminEmail) == null)
                    {
                        var admin = new AppUser
                        {
                            FullName = "Admin-User",
                            Email = adminEmail,
                            UserName = adminEmail,
                            EmailConfirmed = true,
                            CreatedAt = DateTime.UtcNow
                        };
                        var adminPassword = "Admin@1234";
                        await userManager.CreateAsync(admin, adminPassword);
                        await userManager.AddToRoleAsync(admin, roles[0]);
                    }

                   
                    
                }
                catch (Exception ex)
                {
                    // Log the error (you can integrate a logging library here)
                    Console.WriteLine($"An error occurred during database seeding: {ex.Message}");
                }
            }
        }
    }
}
