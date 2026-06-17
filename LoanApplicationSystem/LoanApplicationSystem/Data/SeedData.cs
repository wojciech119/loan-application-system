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

            var adminEmail = "admin@app.pl";
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

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (!result.Succeeded)
                {
                    throw new Exception("Nie udało się utworzyć konta administratora: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, "Administrator"))
            {
                await userManager.AddToRoleAsync(admin, "Administrator");
            }

            var employeeEmail = "pracownik@app.pl";
            var employeePassword = "Pracownik123!";

            var employee = await userManager.FindByEmailAsync(employeeEmail);

            if (employee == null)
            {
                employee = new IdentityUser
                {
                    UserName = employeeEmail,
                    Email = employeeEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(employee, employeePassword);

                if (!result.Succeeded)
                {
                    throw new Exception("Nie udało się utworzyć konta pracownika: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(employee, "Pracownik"))
            {
                await userManager.AddToRoleAsync(employee, "Pracownik");
            }

            if (!context.CmsPages.Any())
            {
                context.CmsPages.Add(
                    new CmsPage
                    {
                        Key = "home",
                        Title = "Ogłoszenie dla pracowników",
                        Content = "Przed zapisaniem wniosku sprawdź poprawność danych klienta oraz kompletność informacji finansowych."
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}