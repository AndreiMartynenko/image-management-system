using System;
using System.ComponentModel.DataAnnotations;

namespace HealthcareIMS.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public int InvoiceId { get; set; } // Relationship to Invoice

        [Required]
        public decimal Amount { get; set; } // Payment amount

        [Required]
        public string PaymentMethod { get; set; } // Payment method (e.g., Cash, Credit Card, Insurance)

        [Required]
        public string PaymentStatus { get; set; } = "Completed"; // Payment status (e.g., Completed)

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // Relationship to Invoice
        public Invoice Invoice { get; set; }
    }
}
