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

            // نقش‌ها
            string[] roles = new string[] { "Admin", "Reception", "Doctor", "Radiologist", "Accountant" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // ساخت کاربر ادمین پیش‌فرض
            var adminEmail = "admin@abc.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "123456aA@");
                if (result.Succeeded)
                {
                    // نقش Admin را به او اضافه کن
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
