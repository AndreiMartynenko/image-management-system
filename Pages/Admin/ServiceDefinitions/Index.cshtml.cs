using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HealthcareIMS.Pages.Admin.ServiceDefinitions
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ServiceDefinition> ServiceDefinitionList { get; set; } = new();

        public async Task OnGetAsync()
        {
            ServiceDefinitionList = await _context.ServiceDefinitions
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}
