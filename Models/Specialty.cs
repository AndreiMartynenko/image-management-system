using System.ComponentModel.DataAnnotations;

namespace HealthcareIMS.Models
{
    public class Specialty
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
