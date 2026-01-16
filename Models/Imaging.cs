using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareIMS.Models
{
    public class Imaging
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VisitId { get; set; }
        [ForeignKey(nameof(VisitId))]
        public Visit Visit { get; set; }

        public string ImagingType { get; set; } // MRI, XRay, ...
        public string DiseaseType { get; set; }

        public string? ImageFile { get; set; }   // مسیر فایل
        public string? Resolution { get; set; }
        public long FileSize { get; set; }
        public string? FileType { get; set; }
        public DateTime CaptureDate { get; set; } = DateTime.Now;

        public string? CapturedBy { get; set; }  // چه کسی (RadiologistUserId؟)
        public string? DeviceID { get; set; }
        public string? BodyPart { get; set; }
        public string? ImagingReason { get; set; }
        public string? InitialAnalysis { get; set; }
        public string? ReportStatus { get; set; } = "Draft"; // Draft, Final
        public string? ConfidentialityLevel { get; set; }
        public bool ConsentProvided { get; set; }
        public string? Comments { get; set; }
    }
}
