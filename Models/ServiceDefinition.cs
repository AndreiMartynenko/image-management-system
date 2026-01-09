using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareIMS.Models
{
    public class ServiceDefinition
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999999)]
        public decimal DefaultCost { get; set; }

        // آیا این سرویس فعال است یا غیرفعال (مثلا deprecate شده)
        public bool IsActive { get; set; } = true;
    }
}
