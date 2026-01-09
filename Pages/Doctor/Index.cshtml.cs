using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Service> DoctorServices { get; set; }

        public async Task OnGetAsync()
        {
            // فقط سرویس‌هایی که ServiceCategory = "DoctorVisit"
            DoctorServices = await _context.Services
                .Include(s => s.Patient).ThenInclude(p => p.User)
                .Include(s => s.Doctor).ThenInclude(d => d.User)
                .Where(s => s.ServiceCategory == "DoctorVisit")  // فیلتر دپارتمان Doctor
                .OrderByDescending(s => s.ServiceDate)
                .ToListAsync();
        }
    }
}
