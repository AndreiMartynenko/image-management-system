using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareIMS.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("VisitId")]
        public Visit Visit { get; set; }
        public int VisitId { get; set; } // Relationship to Visit

        [Required]
        public decimal TotalAmount { get; set; } // Total invoice amount

        [Required]
        public decimal PaidAmount { get; set; } = 0; // Paid amount

        [Required]
        public string PaymentStatus { get; set; } = "Unpaid"; // Payment status (Unpaid, Partially Paid, Paid)

        public DateTime IssuedDate { get; set; } = DateTime.Now;

        // Relationship to Payments
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
