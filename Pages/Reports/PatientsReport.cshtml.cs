using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Reports
{
    [Authorize(Roles = "Accountant,Admin")]
    public class PatientsReportModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public PatientsReportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PatientStatsDto> PatientStats { get; set; }

        public class PatientStatsDto
        {
            public int PatientId { get; set; }
            public string PatientName { get; set; }
            public string Gender { get; set; } // اگر در Patient یا User ذخیره می‌کنید
            public int VisitCount { get; set; }
        }

        public async Task OnGetAsync()
        {
            // کوئری برای گرفتن همهٔ بیماران + تعداد ویزیت
            var query = await _context.Patients
                .Include(p => p.User)
                .Select(p => new PatientStatsDto
                {
                    PatientId = p.Id,
                    PatientName = p.User.FirstName + " " + p.User.LastName,
                    Gender = p.User.Gender, // اگر دارید
                    VisitCount = _context.Visits.Count(v => v.PatientId == p.Id)
                })
                .ToListAsync();

            PatientStats = query;
        }
    }
}
