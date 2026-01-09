using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareIMS.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        [Column(TypeName = "decimal(18,2)")]

        public DateTime InvoiceDate { get; set; }

        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } // Paid, Unpaid, Partial, ...

        // ارتباط با بیمار
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        // اگر خواستید به Appointment هم لینک دهید:
        // public int? AppointmentId { get; set; }
        // public Appointment Appointment { get; set; }
    }
}
