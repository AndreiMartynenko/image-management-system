using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthcareIMS.Pages.Admin.ServiceDefinitions
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ServiceDefinition ServiceDef { get; set; }

        public void OnGet()
        {
            // Just load the form
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // show validation errors
                return Page();
            }

            _context.ServiceDefinitions.Add(ServiceDef);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/ServiceDefinitions/Index");
        }
    }
}
