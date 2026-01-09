using HealthcareIMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Admin.Dashboard
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int ActivePatientsCount { get; set; }
        public int ActiveDoctorsCount { get; set; }
        public decimal PaymentsSumThisMonth { get; set; }

        public async Task OnGet()
        {
            // شمارش بیماران فعال از AspNetUsers
            ActivePatientsCount = await _context.Patients
                .Include(p => p.User) // ارتباط با AspNetUsers
                .CountAsync(p => p.User.Status == "Active");

            // تعداد پزشکان فعال (هنوز جدول Doctors تعریف نشده)
            ActiveDoctorsCount = 5;

            // مجموع پرداخت‌های ماه جاری
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var paymentAmounts = await _context.Payments
                .Where(pm => pm.PaymentDate >= startOfMonth)
                .Select(pm => pm.Amount)
                .ToListAsync();

            PaymentsSumThisMonth = paymentAmounts.Sum();
        }

    }
}
