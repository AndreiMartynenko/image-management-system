using System;
using System.Collections.Generic;

namespace HealthcareIMS.Models
{
    /// <summary>
    /// کلاس پایه برای تمام افرادی که در سیستم وجود دارند (بیمار، پزشک، رادیولوژیست و ...)
    /// </summary>
    public class Person
    {
        public int PersonId { get; set; } // کلید اصلی در دیتابیس

        public string FullName { get; set; }
        public string NationalId { get; set; }      // مثلاً شماره ملی یا شناسه یکتا
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // فیلدهای مشترک دیگر...
    }
}
