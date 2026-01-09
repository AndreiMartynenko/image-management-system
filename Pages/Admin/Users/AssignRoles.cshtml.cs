using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class AssignRolesModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AssignRolesModel(UserManager<User> userManager,
                                RoleManager<IdentityRole> roleManager,
                                ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public User UserData { get; set; }
        public List<IdentityRole> AllRoles { get; set; }

        [BindProperty]
        public List<string> SelectedRoles { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            UserData = user;
            AllRoles = _roleManager.Roles.OrderBy(r => r.Name).ToList();

            var userRoles = await _userManager.GetRolesAsync(user);
            SelectedRoles = userRoles.ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            AllRoles = _roleManager.Roles.OrderBy(r => r.Name).ToList();
            var currentRoles = await _userManager.GetRolesAsync(user);

            // حذف نقش‌هایی که دیگر تیک ندارند
            var removed = currentRoles.Where(r => !SelectedRoles.Contains(r)).ToList();
            if (removed.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, removed);
            }

            // اضافه کردن نقش‌هایی که جدیداً تیک خورده
            var added = SelectedRoles.Where(r => !currentRoles.Contains(r)).ToList();
            if (added.Any())
            {
                await _userManager.AddToRolesAsync(user, added);

                // اگر نقش Doctor اضافه شد
                if (added.Contains("Doctor"))
                {
                    bool docExists = _context.Doctors.Any(d => d.UserId == user.Id);
                    if (!docExists)
                    {
                        var newDoc = new HealthcareIMS.Models.Doctor
                        {
                            UserId = user.Id,
                            Specialization = "Unknown"
                        };
                        _context.Doctors.Add(newDoc);
                        await _context.SaveChangesAsync();
                    }
                }

                // اگر نقش Patient اضافه شد
                if (added.Contains("Patient"))
                {
                    bool patExists = _context.Patients.Any(p => p.UserId == user.Id);
                    if (!patExists)
                    {
                        var newPat = new Patient
                        {
                            UserId = user.Id
                        };
                        _context.Patients.Add(newPat);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToPage("/Admin/Users/Index");
        }
    }
}
