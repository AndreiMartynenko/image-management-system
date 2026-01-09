using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HealthcareIMS.Pages.Reception
{
    [Authorize(Roles = "Reception")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Stats cards
        public int TodayPatientCount { get; set; }
        public int TodayVisitCount { get; set; }

        // Search
        public string SearchTerm { get; set; }

        // Patient list
        public List<PatientViewDto> Patients { get; set; } = new();

        public class PatientViewDto
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public DateTime? LastVisitDate { get; set; }
        }

        public async Task OnGetAsync(string searchTerm)
        {
            SearchTerm = searchTerm ?? "";

            // Today's stats
            var today = DateTime.Today;

            // Count distinct patients who had a visit today
            TodayPatientCount = await _context.Visits
                .Where(v => v.VisitDate.Date == today)
                .Select(v => v.PatientId)
                .Distinct()
                .CountAsync();

            // Count total visits today
            TodayVisitCount = await _context.Visits
                .CountAsync(v => v.VisitDate.Date == today);

            // Patient search
            bool isNumber = int.TryParse(SearchTerm, out int pid);


            var query = _context.Patients
                .Include(p => p.User) // Load related User data for the patient
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                if (isNumber)
                {
                    query = query.Where(p => p.Id == pid ||
                                             p.User.FirstName.Contains(SearchTerm) ||
                                             p.User.LastName.Contains(SearchTerm));
                }
                else
                {
                    query = query.Where(p => p.User.FirstName.Contains(SearchTerm) ||
                                             p.User.LastName.Contains(SearchTerm));
                }
            }

            var patientList = await query
                .OrderByDescending(p => p.Id)
                .Take(50)
                .ToListAsync();


            // Build DTOs for display and last visit info
            var result = new List<PatientViewDto>();

            foreach (var p in patientList)
            {
                // Find the patient's latest visit
                var lastVisit = await _context.Visits
                    .Include(v => v.Patient)
                    .Where(v => v.Patient.Id == p.Id)
                    .OrderByDescending(v => v.VisitDate)
                    .FirstOrDefaultAsync();

                var dto = new PatientViewDto
                {
                    Id = p.Id,
                    FullName = $"{p.User.FirstName} {p.User.LastName}", // Using FirstName and LastName from AspNetUsers
                    LastVisitDate = lastVisit?.VisitDate
                };

                result.Add(dto);
            }



            Patients = result;
        }
    }
}
