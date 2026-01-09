using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HealthcareIMS.Pages.Radiologist
{
    [Authorize(Roles = "Radiologist")]
    public class CaptureImageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CaptureImageModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public int AppointmentId { get; set; }

        [BindProperty]
        public string ImagingType { get; set; }

        [BindProperty]
        public string DiseaseType { get; set; }

        [BindProperty]
        public string BodyPart { get; set; }

        [BindProperty]
        public string Comments { get; set; }

        public Appointment AppointmentData { get; set; }
        public string PatientName { get; set; }

        public async Task<IActionResult> OnGetAsync(int appointmentId)
        {
            AppointmentId = appointmentId;
            AppointmentData = await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (AppointmentData == null)
            {
                return NotFound();
            }

            PatientName = AppointmentData.Patient?.User?.FirstName + " " + AppointmentData.Patient?.User?.LastName;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile ImageFile)
        {
            // Basic validation
            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError("", "Please select an image file.");
                return Page();
            }

            // AppointmentId is read from the BindProperty
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == AppointmentId);

            if (appointment == null)
            {
                return NotFound();
            }

            // If VisitId is stored on Appointment:
            // var visitId = appointment.VisitId (if you have it)
            // Otherwise, you need to resolve the patient's visit

            // Find the real VisitId. If it's on Appointment, great;
            // if not, you can:
            // var openVisit = _context.Visits.FirstOrDefault(v => v.PatientId == appointment.PatientId && v.VisitStatus=="Open");
            // یا ...
            // Here we assume we won't leave VisitId as 0.
            // If you intend to allow Imaging without a required VisitId, you can skip this.
            int visitId = 0;
            var existingVisit = await _context.Visits
                .FirstOrDefaultAsync(v => v.PatientId == appointment.PatientId && v.VisitStatus == "Open");
            // If not found, create a new Visit
            if (existingVisit == null)
            {
                var newVisit = new Visit
                {
                    PatientId = appointment.PatientId,
                    VisitDate = DateTime.Now,
                    VisitStatus = "Open"
                };
                _context.Visits.Add(newVisit);
                await _context.SaveChangesAsync();
                visitId = newVisit.Id;
            }
            else
            {
                visitId = existingVisit.Id;
            }

            // Image storage path
            var uploadsFolder = Path.Combine("wwwroot", "uploads", "imaging");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Create a unique filename
            var uniqueFileName = $"{Guid.NewGuid()}_{ImageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file on server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(fileStream);
            }

            // Create a record in the Imagings table
            var imaging = new Imaging
            {
                VisitId = visitId,
                ImagingType = ImagingType,
                DiseaseType = DiseaseType,
                ImageFile = $"/uploads/imaging/{uniqueFileName}",
                FileSize = ImageFile.Length,
                FileType = ImageFile.ContentType,
                CaptureDate = DateTime.Now,
                CapturedBy = User.Identity.Name, // Radiologist
                BodyPart = BodyPart,
                Comments = Comments,
                ReportStatus = "Draft",
                ConfidentialityLevel = "Normal",
                ConsentProvided = true
            };

            // If needed, you can populate additional fields
            // imaging.ImagingReason = ...
            // imaging.Resolution = ...

            _context.Imagings.Add(imaging);
            await _context.SaveChangesAsync();

            // Redirect back to a page that shows imagings, or Radiologist/Index
            return RedirectToPage("/Radiologist/Index");
        }
    }
}
