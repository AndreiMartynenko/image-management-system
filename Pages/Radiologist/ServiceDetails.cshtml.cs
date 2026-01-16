using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;

namespace HealthcareIMS.Pages.Radiologist
{
    [Authorize(Roles = "Radiologist")]
    public class ServiceDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ServiceDetailsModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Service ServiceData { get; set; }
        public string PatientName { get; set; }
        public List<Service> ServicesInSameVisit { get; set; }
        public List<Imaging> ImagingList { get; set; }

        [BindProperty]
        public string UpdatedDiagnosis { get; set; }
        [BindProperty]
        public string UpdatedComments { get; set; }

        [BindProperty]
        public string SelectedCategory { get; set; }
        [BindProperty]
        public int? SelectedServiceDefinitionId { get; set; }
        public List<string> AllCategories { get; set; }
        public List<ServiceDefinition> FilteredServiceDefinitions { get; set; }

        public async Task<IActionResult> OnGetAsync(int serviceId)
        {
            var userId = _userManager.GetUserId(User);

            // نقش Radiologist
            if (!User.IsInRole("Radiologist"))
                return Forbid();

            var svc = await _context.Services
                .Include(s => s.Patient).ThenInclude(p => p.User)
                .Include(s => s.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(s => s.Id == serviceId);
            if (svc == null)
                return NotFound();

            // اگر سرویس Category != "Radiology" -> forbid
            if (svc.ServiceCategory != "Radiology")
            {
                return Forbid();
            }

            ServiceData = svc;
            PatientName = svc.Patient.User.FirstName + " " + svc.Patient.User.LastName;
            UpdatedDiagnosis = svc.Diagnosis;
            UpdatedComments = svc.Comments;

            ServicesInSameVisit = await _context.Services
                .Include(x => x.Doctor).ThenInclude(d => d.User)
                .Where(x => x.VisitId == svc.VisitId && x.Id != svc.Id)
                .OrderBy(x => x.ServiceDate)
                .ToListAsync();

            ImagingList = await _context.Imagings
                .Where(img => img.VisitId == svc.VisitId)
                .ToListAsync();

            await LoadCategories();
            return Page();
        }

        private async Task LoadCategories()
        {
            var defs = await _context.ServiceDefinitions
                .Where(sd => sd.IsActive)
                .ToListAsync();

            AllCategories = defs
                .Select(sd => sd.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            if (!string.IsNullOrEmpty(SelectedCategory))
            {
                FilteredServiceDefinitions = defs
                    .Where(sd => sd.Category == SelectedCategory)
                    .OrderBy(sd => sd.Name)
                    .ToList();
            }
            else
            {
                FilteredServiceDefinitions = new List<ServiceDefinition>();
            }
        }

        // آپدیت Diagnosis/Comments
        public async Task<IActionResult> OnPostUpdateDiagnosisAsync(int serviceId)
        {
            var userId = _userManager.GetUserId(User);

            var svc = await _context.Services.FindAsync(serviceId);
            if (svc == null)
                return NotFound();

            // کنترل Category
            if (svc.ServiceCategory != "Radiology")
            {
                return Forbid();
            }

            svc.Diagnosis = UpdatedDiagnosis;
            svc.Comments = UpdatedComments;

            _context.Services.Update(svc);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { serviceId });
        }

        // FinishVisit
        public async Task<IActionResult> OnPostFinishVisitAsync(int serviceId)
        {
            var svc = await _context.Services.FindAsync(serviceId);
            if (svc == null)
                return NotFound();

            if (svc.ServiceCategory != "Radiology")
            {
                return Forbid();
            }

            svc.ServiceStatus = "Done";
            _context.Services.Update(svc);

            var visit = await _context.Visits.FindAsync(svc.VisitId);
            if (visit != null)
            {
                visit.VisitStatus = "Closed";
                _context.Visits.Update(visit);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Radiologist/Index");
        }

        // تغییر Category onsubmit
        public async Task<IActionResult> OnPost(int serviceId, string dummy = null)
        {
            return await OnGetAsync(serviceId);
        }

        // آپلود تصویر
        public async Task<IActionResult> OnPostUploadImageAsync(int serviceId, IFormFile ImageFile)
        {
            var svc = await _context.Services.FindAsync(serviceId);
            if (svc == null) return NotFound();

            if (svc.ServiceCategory != "Radiology")
            {
                return Forbid();
            }

            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError("", "Please select an image file.");
                return await ReloadPage(serviceId);
            }

            var uploadsFolder = Path.Combine("wwwroot", "uploads", "imaging");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueName = $"{Guid.NewGuid()}_{ImageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueName);
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(fs);
            }

            var imaging = new Imaging
            {
                VisitId = svc.VisitId,
                ImagingType = "Unknown",
                DiseaseType = "Unknown", // ستون NOT NULL
                ImageFile = $"/uploads/imaging/{uniqueName}",
                CaptureDate = DateTime.Now,
                CapturedBy = User.Identity.Name,
                FileSize = ImageFile.Length,
                FileType = ImageFile.ContentType,
                ReportStatus = "Draft",
                ConfidentialityLevel = "Normal"
            };
            _context.Imagings.Add(imaging);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { serviceId });
        }

        // ReferConfirm
        public async Task<IActionResult> OnPostReferConfirmAsync(int serviceId)
        {
            var svc = await _context.Services.FindAsync(serviceId);
            if (svc == null)
                return NotFound();

            if (svc.ServiceCategory != "Radiology")
            {
                return Forbid();
            }

            svc.ServiceStatus = "Done";
            _context.Services.Update(svc);

            if (string.IsNullOrEmpty(SelectedCategory) || !SelectedServiceDefinitionId.HasValue)
            {
                ModelState.AddModelError("", "Please select Department and ServiceDefinition.");
                return await ReloadPage(serviceId);
            }

            var sd = await _context.ServiceDefinitions.FindAsync(SelectedServiceDefinitionId.Value);
            if (sd == null)
            {
                ModelState.AddModelError("", "Selected ServiceDefinition not found.");
                return await ReloadPage(serviceId);
            }

            var newService = new Service
            {
                VisitId = svc.VisitId,
                PatientId = svc.PatientId,
                ServiceName = sd.Name,
                ServiceCategory = sd.Category,
                ServiceCost = sd.DefaultCost,
                ServiceDate = DateTime.Now,
                ServiceStatus = "Pending",
                Comments = "Referred by Radiologist"
            };
            _context.Services.Add(newService);

            var visit = await _context.Visits.FindAsync(svc.VisitId);
            if (visit != null)
            {
                if (visit.TotalCost == null) visit.TotalCost = 0;
                visit.TotalCost += sd.DefaultCost;
                _context.Visits.Update(visit);

                var inv = await _context.Invoices.FirstOrDefaultAsync(i => i.VisitId == visit.Id);
                if (inv == null)
                {
                    inv = new Invoice
                    {
                        VisitId = visit.Id,
                        TotalAmount = 0,
                        PaymentStatus = "Unpaid",
                        IssuedDate = DateTime.Now
                    };
                    _context.Invoices.Add(inv);
                }
                inv.TotalAmount += sd.DefaultCost;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Radiologist/Index");
        }

        private async Task<IActionResult> ReloadPage(int serviceId)
        {
            await OnGetAsync(serviceId);
            return Page();
        }
    }
}
