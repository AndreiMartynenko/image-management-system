using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Reports
{
    [Authorize(Roles = "Admin,Doctor,Radiologist,Accountant,Reception")]
    // یا هر نقشی که اجازه دسترسی به گزارش را دارد
    public class PatientHistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PatientHistoryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int? PatientId { get; set; }

        public Patient TargetPatient { get; set; }
        public List<Visit> Visits { get; set; }
        public List<Service> Services { get; set; }

        public async Task OnGetAsync()
        {
            if (PatientId.HasValue)
            {
                TargetPatient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == PatientId.Value);

                if (TargetPatient != null)
                {
                    // تمام Visits
                    Visits = await _context.Visits
                        .Where(v => v.PatientId == TargetPatient.Id)
                        .OrderByDescending(v => v.Id)
                        .ToListAsync();

                    // تمام Services
                    Services = await _context.Services
                        .Where(s => s.PatientId == TargetPatient.Id)
                        .Include(s => s.Doctor).ThenInclude(d => d.User)
                        .OrderByDescending(s => s.ServiceDate)
                        .ToListAsync();
                }
            }
        }
    }
}
