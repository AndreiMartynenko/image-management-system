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
        public int VisitId { get; set; } // ارتباط با Visit

        [Required]
        public decimal TotalAmount { get; set; } // مجموع مبلغ صورتحساب

        [Required]
        public decimal PaidAmount { get; set; } = 0; // مبلغ پرداخت‌شده

        [Required]
        public string PaymentStatus { get; set; } = "Unpaid"; // وضعیت پرداخت (Unpaid, Partially Paid, Paid)

        public DateTime IssuedDate { get; set; } = DateTime.Now;

        // ارتباط با Payments
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
