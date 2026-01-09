using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthcareIMS.Pages.Admin.Roles
{
    public class CreateModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [BindProperty]
        public string RoleName { get; set; } // فقط نام نقش را می‌خواهیم

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(RoleName))
            {
                ModelState.AddModelError(string.Empty, "Role name is required.");
                return Page();
            }

            // ساخت نقش جدید
            var normalizedName = RoleName.ToUpper(); // برای NormalizedName
            var role = new IdentityRole
            {
                Name = RoleName,
                NormalizedName = normalizedName
            };

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToPage("/Admin/Roles/Index");
            }

            // اگر مشکلی بود، خطاها را به ModelState اضافه کنید
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
