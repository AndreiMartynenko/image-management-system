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
    public class VisitsReportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public VisitsReportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // فیلدهای فیلتر
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public List<Visit> Visits { get; set; }

        public async Task OnGetAsync()
        {
            // کوئری پایه
            var query = _context.Visits
                .Include(v => v.Patient)
                    .ThenInclude(p => p.User)
                .AsQueryable();

            // اگر StartDate و EndDate تعیین شده، روی VisitDate فیلتر می‌کنیم
            if (StartDate.HasValue)
            {
                query = query.Where(v => v.VisitDate >= StartDate.Value);
            }
            if (EndDate.HasValue)
            {
                // معمولاً EndDate رو تا انتهای روز در نظر می‌گیرند
                var end = EndDate.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(v => v.VisitDate <= end);
            }

            Visits = await query.OrderByDescending(v => v.VisitDate).ToListAsync();
        }
    }
}
