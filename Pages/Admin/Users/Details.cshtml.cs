using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthcareIMS.Data;
using HealthcareIMS.Models;
using System.Linq;
using System.Threading.Tasks;
using HealthcareIMS.Data;
using HealthcareIMS.Models;

namespace HealthcareIMS.Pages.Admin.Users
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public User UserData { get; set; }
        public string RolesString { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            UserData = await _context.Users.FindAsync(id);
            if (UserData == null) return NotFound();

            var roleIds = await _context.UserRoles
                .Where(ur => ur.UserId == id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var roleNames = await _context.Roles
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToListAsync();

            RolesString = string.Join(", ", roleNames);

            return Page();
        }
    }
}
