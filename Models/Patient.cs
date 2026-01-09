using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareIMS.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; } // Primary key

        [ForeignKey("User")]
        public string UserId { get; set; } // Foreign key to AspNetUsers(Id)

        // Relationship to AspNetUsers
        public virtual User User { get; set; } // Navigation Property
    }
}
