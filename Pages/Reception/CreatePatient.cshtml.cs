using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Reception
{
    [Authorize(Roles = "Reception")]
    public class CreatePatientModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        // private readonly RoleManager<IdentityRole> _roleManager; // اگر لازم شد

        public CreatePatientModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            public string PhoneNumber { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string Address { get; set; }
        }

        public void OnGet()
        {
            // show empty form
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // می‌توانید پسورد را به‌صورت پیش‌فرض بسازید
            var defaultPassword = "DefaultPassword123!";

            var user = new User
            {
                UserName = Input.Email, // یا هرچیز دیگر
                Email = Input.Email,
                PhoneNumber = Input.PhoneNumber,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                DateOfBirth = Input.DateOfBirth,
                Gender = Input.Gender,
                Address = Input.Address,
                Status = "Active"
            };

            // ساخت کاربر در Identity
            var result = await _userManager.CreateAsync(user, defaultPassword);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, e.Description);
                }
                return Page();
            }

            // افزودن نقش Patient
            await _userManager.AddToRoleAsync(user, "Patient");

            // ساخت رکورد در جدول Patients
            var newPatient = new Patient
            {
                UserId = user.Id
            };
            _context.Patients.Add(newPatient);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Reception/Index");
        }
    }
}
