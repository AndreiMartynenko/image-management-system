using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Reports
{
    [Authorize(Roles = "Accountant,Admin")]
    public class DoctorPerformanceModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public DoctorPerformanceModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public List<DoctorStatsDto> DoctorStats { get; set; }

        public class DoctorStatsDto
        {
            public string DoctorName { get; set; }
            public string Specialization { get; set; }
            public int ServiceCount { get; set; }
            public decimal TotalCost { get; set; }
        }

        public async Task OnGetAsync()
        {
            var query = _context.Services
                .Include(s => s.Doctor)
                    .ThenInclude(d => d.User)
                .AsQueryable();

            if (StartDate.HasValue)
            {
                query = query.Where(s => s.ServiceDate >= StartDate.Value);
            }
            if (EndDate.HasValue)
            {
                var end = EndDate.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(s => s.ServiceDate <= end);
            }

            // گروه‌بندی بر اساس DoctorId
            var grouped = await query
                .GroupBy(s => s.DoctorId)
                .Select(g => new
                {
                    DoctorId = g.Key,
                    ServiceCount = g.Count(),
                    TotalCost = g.Sum(x => x.ServiceCost)
                })
                .ToListAsync();

            // حالا اطلاعات دکتر را ضمیمه می‌کنیم
            DoctorStats = new List<DoctorStatsDto>();
            foreach (var grp in grouped)
            {
                if (grp.DoctorId == null)
                    continue; // ممکن است برخی سرویس‌ها بدون دکتر ثبت شده باشند

                var doc = await _context.Doctors
                    .Include(d => d.User)
                    .FirstOrDefaultAsync(d => d.Id == grp.DoctorId);

                if (doc != null)
                {
                    DoctorStats.Add(new DoctorStatsDto
                    {
                        DoctorName = doc.User.FirstName + " " + doc.User.LastName,
                        Specialization = doc.Specialization,
                        ServiceCount = grp.ServiceCount,
                        TotalCost = grp.TotalCost
                    });
                }
            }
        }
    }
}
