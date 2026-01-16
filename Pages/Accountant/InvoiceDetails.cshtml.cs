using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Accountant
{
    [Authorize(Roles = "Accountant")]
    public class InvoiceDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public InvoiceDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Invoice Invoice { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Invoice = await _context.Invoices
                .Include(i => i.Payments)
                .Include(i => i.Visit)
                    .ThenInclude(v => v.Services)
                        .ThenInclude(s => s.Doctor)
                            .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (Invoice == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, decimal amount, string paymentMethod)
        {
            // فاکتور را از دیتابیس بخوانیم
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            // محاسبه‌ی مبلغ باقیمانده
            var remaining = invoice.TotalAmount - invoice.PaidAmount;
            if (amount > remaining)
            {
                // اگر مبلغ پرداخت بیش از مبلغ باقیمانده باشد، خطا بده
                ModelState.AddModelError(string.Empty,
                    "The payment amount cannot exceed the remaining invoice balance.");

                // برای نمایش مجدد داده‌ها در صفحه، دوباره Invoice را بارگیری می‌کنیم
                Invoice = await _context.Invoices
                    .Include(i => i.Payments)
                    .Include(i => i.Visit)
                        .ThenInclude(v => v.Services)
                            .ThenInclude(s => s.Doctor)
                                .ThenInclude(d => d.User)
                    .FirstOrDefaultAsync(i => i.Id == id);

                return Page();
            }

            // ایجاد یک رکورد پرداخت جدید
            var payment = new Payment
            {
                InvoiceId = id,
                Amount = amount,
                PaymentMethod = paymentMethod
            };
            _context.Payments.Add(payment);

            // به‌روزرسانی وضعیت فاکتور
            invoice.PaidAmount += amount;
            if (invoice.PaidAmount >= invoice.TotalAmount)
            {
                invoice.PaymentStatus = "Paid";
            }
            else
            {
                invoice.PaymentStatus = "Partially Paid";
            }

            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            // پس از موفقیت در پرداخت، بازهم به همین صفحه برمی‌گردیم
            return RedirectToPage(new { id });
        }
    }
}
