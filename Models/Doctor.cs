using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareIMS.Models
{
    public class Doctor
    {
        [Key]
        public int? Id { get; set; } // Primary key

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } // Foreign key to AspNetUsers(Id)

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; } // Specialization

        [StringLength(15)]
        public string? ContactNumber { get; set; } // Optional contact number

        // Optional fields such as license and degree
        public string?   LicenseNo { get; set; }
        public string? Degree { get; set; }

        // Relationship to AspNetUsers
        public virtual User User { get; set; } // Navigation Property
    }
}
