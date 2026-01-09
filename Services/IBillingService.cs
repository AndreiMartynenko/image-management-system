using HealthcareIMS.Models;

namespace HealthcareIMS.Services
{
    public interface IBillingService
    {
        Task<Invoice?> GetInvoiceDetailsAsync(int id);
        Task<(bool Success, string? ErrorMessage)> AddPaymentAsync(int invoiceId, decimal amount, string paymentMethod);
    }
}
