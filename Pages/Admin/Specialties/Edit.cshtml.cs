using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthcareIMS.Pages.Admin.Specialties
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var spec = await _context.Specialties.FindAsync(SpecialtyData.Id);
            if (spec == null)
            {
                return NotFound();
            }

            spec.Name = SpecialtyData.Name;
            spec.Description = SpecialtyData.Description;
            spec.IsActive = SpecialtyData.IsActive;

            await _context.SaveChangesAsync();
            return RedirectToPage("/Admin/Specialties/Index");
        }
    }
}
