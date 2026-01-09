using System.Collections.Generic;

namespace HealthcareIMS.Models
{
    public class Doctor : Person
    {
        // چون از Person ارث‌بری می‌کنیم، DoctorId حذف شده 
        // (همان PersonId به عنوان کلید اصلی استفاده می‌شود)

        public string Specialty { get; set; }      // تخصص (قلب، مغز و اعصاب، داخلی و …)

        // ارتباط با نوبت‌ها
        public ICollection<Appointment> Appointments { get; set; }
    }
}
