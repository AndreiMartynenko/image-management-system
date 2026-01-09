using System;
using System.Collections.Generic;

namespace HealthcareIMS.Models
{
    public class Patient : Person
    {
        public DateTime DateOfBirth { get; set; }   // تاریخ تولد
        public string Conditions { get; set; }      // وضعیت‌های پزشکی / بیماری‌ها

        // لیست نوبت‌هایی که این بیمار دارد
        public ICollection<Appointment> Appointments { get; set; }

        // لیست صورت‌حساب‌ها (فاکتورها)
        public ICollection<Invoice> Invoices { get; set; }

        // لیست تصاویر پزشکی
        public ICollection<MedicalImage> MedicalImages { get; set; }
    }
}
