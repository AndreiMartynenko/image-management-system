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

        // کارت آمار
        public int TodayPatientCount { get; set; }
        public int TodayVisitCount { get; set; }

        // جستجو
        public string SearchTerm { get; set; }

        // لیست بیماران
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

            // آمار امروز
            var today = DateTime.Today;

            // شمارش بیمارانی که امروز ویزیت داشته‌اند
            TodayPatientCount = await _context.Visits
                .Where(v => v.VisitDate.Date == today)
                .Select(v => v.PatientId)
                .Distinct()
                .CountAsync();

            // شمارش کل ویزیت‌های امروز
            TodayVisitCount = await _context.Visits
                .CountAsync(v => v.VisitDate.Date == today);

            // جستجوی بیماران
            bool isNumber = int.TryParse(SearchTerm, out int pid);


            var query = _context.Patients
                .Include(p => p.User) // بارگذاری داده‌های User مرتبط با Patient
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


            // ساخت DTO برای نمایش بیماران و آخرین ویزیت
            var result = new List<PatientViewDto>();

            foreach (var p in patientList)
            {
                // پیدا کردن آخرین ویزیت بیمار
                var lastVisit = await _context.Visits
                    .Include(v => v.Patient)
                    .Where(v => v.Patient.Id == p.Id)
                    .OrderByDescending(v => v.VisitDate)
                    .FirstOrDefaultAsync();

                var dto = new PatientViewDto
                {
                    Id = p.Id,
                    FullName = $"{p.User.FirstName} {p.User.LastName}", // استفاده از FirstName و LastName از AspNetUsers
                    LastVisitDate = lastVisit?.VisitDate
                };

                result.Add(dto);
            }



            Patients = result;
        }
    }
}
