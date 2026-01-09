using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public CreateModel(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            public string PhoneNumber { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string Address { get; set; }
            public string Status { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
            // Just display empty form
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ساخت User شیء
            var user = new User
            {
                UserName = Input.Email, // Username = Email
                Email = Input.Email,
                PhoneNumber = Input.PhoneNumber,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                DateOfBirth = Input.DateOfBirth,
                Gender = Input.Gender,
                Address = Input.Address,
                Status = string.IsNullOrEmpty(Input.Status) ? "Active" : Input.Status
            };
            await _userManager.AddToRoleAsync(user, "Patient");
            // سپس:
            var newPatient = new Patient { UserId = user.Id };
            _context.Patients.Add(newPatient);
            await _context.SaveChangesAsync();

            // ایجاد در Identity و هش پسورد
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, e.Description);
                }
                return Page();
            }

            // به طور پیش‌فرض اینجا نقش خاصی نمی‌دهیم (Admin بعداً در AssignRoles می‌دهد)

            return RedirectToPage("/Admin/Users/Index");
        }
    }
}
