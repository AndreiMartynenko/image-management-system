using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareIMS.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; } // FK to Patients

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        [Required]
        public int? DoctorId { get; set; } // FK to Doctors

        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } // e.g., Pending, Done, Cancelled
    }
}
