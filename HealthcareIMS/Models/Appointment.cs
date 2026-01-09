using System;

namespace HealthcareIMS.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }   // کلید اصلی
        public DateTime ScheduledTime { get; set; }
        public string Status { get; set; }       // وضعیت نوبت: Scheduled, Completed, Canceled, etc.

        // بیمار
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        // پزشک
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        // اگر نیاز داشتید رادیولوژیست هم درج کنید:
        public int? RadiologistId { get; set; }
        public Radiologist Radiologist { get; set; }
    }
}
