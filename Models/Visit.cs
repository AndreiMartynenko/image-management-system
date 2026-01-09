using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareIMS.Models
{
    public class Visit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; } // FK to Patients

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        public DateTime VisitDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string VisitStatus { get; set; } // e.g., Open, Closed
        public string? Notes { get; set; }
        public decimal TotalCost { get; set; }
        public ICollection<Service> Services { get; set; } = new List<Service>();

    }
}
