using System;

namespace HealthcareIMS.Models
{
    public class MedicalImage
    {
        public int MedicalImageId { get; set; }
        public string ImageType { get; set; }   // MRI, CT, XRay
        public string FilePath { get; set; }    // محل ذخیره فایل
        public DateTime UploadDate { get; set; }
        public string Description { get; set; }

        // رادیولوژیست آپلودکننده
        public int RadiologistId { get; set; }
        public Radiologist Radiologist { get; set; }

        // بیماری که تصویر مربوط به اوست
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
