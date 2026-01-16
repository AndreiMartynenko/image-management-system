using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareIMS.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; } // کلید اصلی

        [ForeignKey("User")]
        public string UserId { get; set; } // کلید خارجی به AspNetUsers(Id)

        // ارتباط با AspNetUsers
        public virtual User User { get; set; } // Navigation Property
    }
}
