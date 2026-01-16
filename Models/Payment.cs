using System;
using System.ComponentModel.DataAnnotations;

namespace HealthcareIMS.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public int InvoiceId { get; set; } // ارتباط با Invoice

        [Required]
        public decimal Amount { get; set; } // مبلغ پرداختی

        [Required]
        public string PaymentMethod { get; set; } // روش پرداخت (مانند Cash, Credit Card, Insurance)

        [Required]
        public string PaymentStatus { get; set; } = "Completed"; // وضعیت پرداخت (مانند Completed)

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // ارتباط با Invoice
        public Invoice Invoice { get; set; }
    }
}
