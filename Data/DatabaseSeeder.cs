using HealthcareIMS.Models;
using Microsoft.AspNetCore.Identity;

namespace HealthcareIMS.Data
{

    public static class DatabaseSeeder
    {
       
        public static async Task SeedRolesAndAdminUserAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // Roles
            string[] roles = new string[] { "Admin", "Reception", "Doctor", "Radiologist", "Accountant", "Patient" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create default admin user
            var adminEmail = "admin@aurora.com";
            var legacyAdminEmail = "admin@abc.com";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var legacyAdminUser = await userManager.FindByEmailAsync(legacyAdminEmail);
                if (legacyAdminUser != null)
                {
                    var setEmailResult = await userManager.SetEmailAsync(legacyAdminUser, adminEmail);
                    var setUserNameResult = await userManager.SetUserNameAsync(legacyAdminUser, adminEmail);
                    if (setEmailResult.Succeeded && setUserNameResult.Succeeded)
                    {
                        legacyAdminUser.EmailConfirmed = true;
                        adminUser = legacyAdminUser;
                        await userManager.UpdateAsync(adminUser);
                    }
                }

                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(adminUser, "123456aA@");
                    if (!result.Succeeded)
                    {
                        return;
                    }
                }
            }

            if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
