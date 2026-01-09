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
    [Authorize(Roles = "Admin,Accountant")]
    public class FinancialReportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public FinancialReportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public List<Invoice> Invoices { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Invoices
                .AsQueryable();

            if (StartDate.HasValue)
            {
                query = query.Where(i => i.IssuedDate >= StartDate.Value);
            }
            if (EndDate.HasValue)
            {
                var endDate = EndDate.Value.AddDays(1).AddSeconds(-1);
                query = query.Where(i => i.IssuedDate <= endDate);
            }

            Invoices = await query
                .OrderByDescending(i => i.IssuedDate)
                .Take(1000) // حداکثر 1000 رکورد فرضی
                .ToListAsync();
        }
    }
}
