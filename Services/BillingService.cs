using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareIMS.Services
{
    public class BillingService : IBillingService
    {
        private readonly ApplicationDbContext _context;

        public BillingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice?> GetInvoiceDetailsAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Payments)
                .Include(i => i.Visit)
                    .ThenInclude(v => v.Services)
                        .ThenInclude(s => s.Doctor)
                            .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<(bool Success, string? ErrorMessage)> AddPaymentAsync(int invoiceId, decimal amount, string paymentMethod)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null)
            {
                return (false, "Invoice not found.");
            }

            var remaining = invoice.TotalAmount - invoice.PaidAmount;
            if (amount > remaining)
            {
                return (false, "The payment amount cannot exceed the remaining invoice balance.");
            }

            var payment = new Payment
            {
                InvoiceId = invoiceId,
                Amount = amount,
                PaymentMethod = paymentMethod
            };
            _context.Payments.Add(payment);

            invoice.PaidAmount += amount;
            invoice.PaymentStatus = invoice.PaidAmount >= invoice.TotalAmount ? "Paid" : "Partially Paid";

            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}
