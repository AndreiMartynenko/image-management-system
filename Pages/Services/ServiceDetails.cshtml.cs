using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Services
{
    [Authorize]
    public class ServiceDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ServiceDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Service ServiceData { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ServiceData = await _context.Services
                .Include(s => s.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (ServiceData == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
