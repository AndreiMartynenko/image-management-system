using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareIMS.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Visit")]
        public int VisitId { get; set; }
        public Visit Visit { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        // DoctorId is nullable
        [ForeignKey("Doctor")]
        public int? DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [StringLength(100)]
        public string ServiceName { get; set; }
        [StringLength(50)]
        public string? ServiceCategory { get; set; }
        public decimal ServiceCost { get; set; }
        public DateTime ServiceDate { get; set; }

        public string? Comments { get; set; }
        [StringLength(20)]
        public string? ServiceStatus { get; set; }
        public string? Diagnosis { get; set; }
    }
}
