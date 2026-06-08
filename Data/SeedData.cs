using LoanApplicationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoanApplicationSystem.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            string[] roles = { "Administrator", "Pracownik" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@loanapp.pl";
            var adminPassword = "Admin123!";

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(admin, adminPassword);
            }

            if (!await userManager.IsInRoleAsync(admin, "Administrator"))
            {
                await userManager.AddToRoleAsync(admin, "Administrator");
            }

            if (!context.CmsPages.Any())
            {
                context.CmsPages.AddRange(
                    new CmsPage
                    {
                        Key = "home",
                        Title = "Przypomnienie dla pracowników",
                        Content = "Przed utworzeniem wniosku sprawdź kompletność danych klienta, miesięczny dochód netto, aktualne zobowiązania oraz cel kredytu."
                    },
                    new CmsPage
                    {
                        Key = "about",
                        Title = "Opis wewnętrznego procesu",
                        Content = "System wspiera pracowników banku w tworzeniu wniosków kredytowych i przekazywaniu ich do analizy przez przełożonego."
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}