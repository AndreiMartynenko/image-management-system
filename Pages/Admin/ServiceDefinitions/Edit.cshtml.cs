using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthcareIMS.Pages.Admin.ServiceDefinitions
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
        public ServiceDefinition ServiceDef { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ServiceDef = await _context.ServiceDefinitions.FindAsync(id);
            if (ServiceDef == null)
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

            var sd = await _context.ServiceDefinitions.FindAsync(ServiceDef.Id);
            if (sd == null)
            {
                return NotFound();
            }

            sd.Name = ServiceDef.Name;
            sd.Category = ServiceDef.Category;
            sd.DefaultCost = ServiceDef.DefaultCost;
            sd.IsActive = ServiceDef.IsActive;

            await _context.SaveChangesAsync();
            return RedirectToPage("/Admin/ServiceDefinitions/Index");
        }
    }
}
