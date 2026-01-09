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
    public class ServicesReportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ServicesReportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public List<Service> ServicesList { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Services
                .Include(s => s.Patient)
                    .ThenInclude(p => p.User)
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

            ServicesList = await query
                .OrderByDescending(s => s.ServiceDate)
                .ToListAsync();
        }
    }
}
