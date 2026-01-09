using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthcareIMS.Pages.Admin.Specialties
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Specialty SpecialtyData { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            SpecialtyData = await _context.Specialties.FindAsync(id);
            if (SpecialtyData == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var spec = await _context.Specialties.FindAsync(id);
            if (spec == null)
            {
                return NotFound();
            }

            _context.Specialties.Remove(spec);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Admin/Specialties/Index");
        }
    }
}
