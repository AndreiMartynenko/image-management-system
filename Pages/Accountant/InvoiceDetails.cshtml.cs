using HealthcareIMS.Data;
using HealthcareIMS.Models;
using HealthcareIMS.Services;
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
        private readonly IBillingService _billingService;

        public InvoiceDetailsModel(IBillingService billingService)
        {
            _billingService = billingService;
        }

        [BindProperty]
        public Invoice Invoice { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Invoice = await _billingService.GetInvoiceDetailsAsync(id);

            if (Invoice == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, decimal amount, string paymentMethod)
        {
            var result = await _billingService.AddPaymentAsync(id, amount, paymentMethod);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Payment failed.");
                Invoice = await _billingService.GetInvoiceDetailsAsync(id);
                if (Invoice == null)
                {
                    return NotFound();
                }
                return Page();
            }

            // After successful payment, return to the same page
            return RedirectToPage(new { id });
        }
    }
}
