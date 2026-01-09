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
    public class InvoicesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public InvoicesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // فیلد جست‌وجو
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public List<Invoice> Invoices { get; set; }

        public async Task OnGetAsync()
        {
            // کوئری پایه
            var query = _context.Invoices
                .Include(i => i.Visit)
                    .ThenInclude(v => v.Patient)
                        .ThenInclude(p => p.User)
                                .Where(i => i.PaymentStatus != "Paid")

                .AsQueryable();

            // اعمال فیلتر بر اساس SearchTerm
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(i =>
                    i.Id.ToString().Contains(SearchTerm) ||
                    i.VisitId.ToString().Contains(SearchTerm) ||
                    (i.Visit.Patient.User.FirstName + " " + i.Visit.Patient.User.LastName).Contains(SearchTerm) ||
                    i.PaymentStatus.Contains(SearchTerm)
                );
            }

            // اجرای کوئری
            Invoices = await query.ToListAsync();
        }
    }
}
