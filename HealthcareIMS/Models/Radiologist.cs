using System.Collections.Generic;
using System;

namespace HealthcareIMS.Models
{
    public class Radiologist : Person
    {
        public string RadiologyCertCode { get; set; }   // مثلاً کد گواهینامه رادیولوژی

        // رابطه با تصاویر پزشکی
        public ICollection<MedicalImage> MedicalImages { get; set; }
    }
}
