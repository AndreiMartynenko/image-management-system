using HealthcareIMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Reports
{
    [Authorize(Roles = "Accountant,Admin")]
    public class ManagementDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ManagementDashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalVisits { get; set; }
        public decimal TotalRevenue { get; set; }
        public int UnpaidInvoicesCount { get; set; }
        public int TotalPatients { get; set; }

        public async Task OnGetAsync()
        {
            // مجموع تعداد ویزیت
            TotalVisits = await _context.Visits.CountAsync();

            // مجموع درآمد (PaidAmount یا به صلاحدید شما)
            // اگر منظور کل صورتحساب هاست:  Sum(TotalAmount) هم می‌توانید محاسبه کنید
            TotalRevenue = await _context.Invoices.SumAsync(i => i.PaidAmount);

            // تعداد فاکتورهایی که هنوز پرداخت کامل نشده‌اند
            UnpaidInvoicesCount = await _context.Invoices
                .CountAsync(i => i.PaymentStatus != "Paid");

            // تعداد بیماران
            TotalPatients = await _context.Patients.CountAsync();
        }
    }
}
