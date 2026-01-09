using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace HealthcareIMS.Helpers
{
    public static class UserManagerHelper
    {
        /// <summary>
        /// افزودن یک کاربر به یک نقش خاص
        /// </summary>
        /// <param name="serviceProvider">سرویس پرووایدر برای دسترسی به UserManager</param>
        /// <param name="email">ایمیل کاربر</param>
        /// <param name="role">نقش مورد نظر</param>
        public static async Task AddUserToRole(IServiceProvider serviceProvider, string email, string role)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(email);

            if (user != null && !await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
