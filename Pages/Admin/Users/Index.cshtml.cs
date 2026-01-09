using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthcareIMS.Data;
using HealthcareIMS.Models;
using System.Linq;

namespace HealthcareIMS.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<User> UsersList { get; set; }

        public async Task OnGetAsync()
        {
            UsersList = await _context.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();
        }

        public string GetRolesString(string userId)
        {
            // برمی‌گرداند لیست نقش‌هایی که این User دارد، جداشده با ویرگول
            var roleIds = _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToList();

            var roleNames = _context.Roles
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToList();

            return string.Join(", ", roleNames);
        }
    }
}
