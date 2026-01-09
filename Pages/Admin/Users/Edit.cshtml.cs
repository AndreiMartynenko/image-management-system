using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthcareIMS.Data;
using HealthcareIMS.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HealthcareIMS.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public User UserData { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            UserData = await _context.Users.FindAsync(id);
            if (UserData == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var userInDb = await _context.Users.FindAsync(UserData.Id);
            if (userInDb == null) return NotFound();

            // ویرایش فیلدها (بجز PasswordHash)
            userInDb.UserName = UserData.UserName;
            userInDb.NormalizedUserName = UserData.UserName?.ToUpper();
            userInDb.Email = UserData.Email;
            userInDb.NormalizedEmail = UserData.Email?.ToUpper();
            userInDb.PhoneNumber = UserData.PhoneNumber;
            userInDb.FirstName = UserData.FirstName;
            userInDb.LastName = UserData.LastName;
            userInDb.DateOfBirth = UserData.DateOfBirth;
            userInDb.Gender = UserData.Gender;
            userInDb.Address = UserData.Address;
            userInDb.Status = UserData.Status;

            // اگر می‌خواهید تغییرات فیلدهای بالا را در Identity هم sync کنید، باید از userManager.UpdateAsync استفاده کنیم
            // اما چون userManager.UpdateAsync هم نهایتاً در AspNetUsers می‌نویسد، می‌توانیم مستقیماً _context.SaveChangesAsync کنیم.
            // اینجا از _context.SaveChangesAsync استفاده می‌کنیم. 

            // رسیدگی به تغییر پسورد
            if (!string.IsNullOrEmpty(NewPassword))
            {
                // پاک کردن رمز فعلی (در صورت وجود)، و افزودن رمز جدید
                var removePassResult = await _userManager.RemovePasswordAsync(userInDb);
                // ممکن است اگر کاربر قبلاً رمز نداشته یا...
                // اینجا اگر شکست خورد می‌توانید هندل کنید اما معمولاً مشکلی نیست.

                var addPassResult = await _userManager.AddPasswordAsync(userInDb, NewPassword);
                if (!addPassResult.Succeeded)
                {
                    foreach (var err in addPassResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, err.Description);
                    }
                    return Page();
                }
            }

            // ذخیرهٔ تغییرات در بقیه فیلدها
            _context.Users.Update(userInDb);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/Users/Index");
        }
    }
}
