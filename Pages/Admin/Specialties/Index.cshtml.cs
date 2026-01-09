using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HealthcareIMS.Pages.Admin.Specialties
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Specialty> SpecialtyList { get; set; } = new();

        public async Task OnGetAsync()
        {
            SpecialtyList = await _context.Specialties
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}
