using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthcareIMS.Pages.Admin.ServiceDefinitions
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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var sd = await _context.ServiceDefinitions.FindAsync(id);
            if (sd == null)
            {
                return NotFound();
            }

            _context.ServiceDefinitions.Remove(sd);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Admin/ServiceDefinitions/Index");
        }
    }
}
