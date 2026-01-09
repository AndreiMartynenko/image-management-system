using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Accountant
{
    [Authorize(Roles = "Accountant")]
    public class AllInvoicesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AllInvoicesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public List<Invoice> Invoices { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Invoices
                .Include(i => i.Visit)
                    .ThenInclude(v => v.Patient)
                        .ThenInclude(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(i =>
                    i.Id.ToString().Contains(SearchTerm) ||
                    i.VisitId.ToString().Contains(SearchTerm) ||
                    (i.Visit.Patient.User.FirstName + " " + i.Visit.Patient.User.LastName).Contains(SearchTerm) ||
                    i.PaymentStatus.Contains(SearchTerm)
                );
            }

            Invoices = await query.ToListAsync();
        }
    }
}
