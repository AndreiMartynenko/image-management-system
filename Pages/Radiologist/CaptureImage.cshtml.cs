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
            // اعتبارسنجی پایه
            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError("", "Please select an image file.");
                return Page();
            }

            // لطفاً AppointmentId از BindProperty خوانده می‌شود
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == AppointmentId);

            if (appointment == null)
            {
                return NotFound();
            }

            // اگر VisitId داخل Appointment هم ذخیره شده باشد:
            // var visitId = appointment.VisitId (اگر دارید)
            // در غیر این صورت باید somehow visit patient

            // در اینجا باید VisitId واقعی را بیابیم. اگر در appointment هست، عالی؛
            // اگر نه، می‌توانید:
            // var openVisit = _context.Visits.FirstOrDefault(v => v.PatientId == appointment.PatientId && v.VisitStatus=="Open");
            // یا ...
            // اینجا ساده فرض می‌کنیم VisitId = 0 نداریم.  
            // یا اگر قصد دارید Imaging بدون VisitId اجباری باشد، می‌توانید Skip کنید.
            int visitId = 0;
            var existingVisit = await _context.Visits
                .FirstOrDefaultAsync(v => v.PatientId == appointment.PatientId && v.VisitStatus == "Open");
            // اگر پیدا نکردیم، می‌توانیم یک Visit ایجاد کنیم یا ...
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

            // مسیر ذخیره‌سازی تصاویر
            var uploadsFolder = Path.Combine("wwwroot", "uploads", "imaging");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // ساخت یک نام فایل یکتا
            var uniqueFileName = $"{Guid.NewGuid()}_{ImageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // ذخیره فایل روی سرور
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(fileStream);
            }

            // ساخت یک record در جدول Imagings
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

            // بسته به نیاز، فیلدهای دیگر را هم می‌توانید مقداردهی کنید
            // imaging.ImagingReason = ...
            // imaging.Resolution = ...

            _context.Imagings.Add(imaging);
            await _context.SaveChangesAsync();

            // می‌توانیم کاربر را به صفحه‌ای ببریم که لیست تصویربرداری‌ها را نشان دهد
            // یا به Radiologist/Index
            return RedirectToPage("/Radiologist/Index");
        }
    }
}
