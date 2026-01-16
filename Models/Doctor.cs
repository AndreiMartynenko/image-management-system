using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareIMS.Models
{
    public class Doctor
    {
        [Key]
        public int? Id { get; set; } // کلید اصلی

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } // کلید خارجی به AspNetUsers(Id)

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; } // تخصص

        [StringLength(15)]
        public string? ContactNumber { get; set; } // شماره تماس اختیاری

        // فیلدهای اختیاری مانند لایسنس و مدرک
        public string?   LicenseNo { get; set; }
        public string? Degree { get; set; }

        // ارتباط با AspNetUsers
        public virtual User User { get; set; } // Navigation Property
    }
}
