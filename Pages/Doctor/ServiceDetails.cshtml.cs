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

namespace HealthcareIMS.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
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

            // Doctor role
            if (!User.IsInRole("Doctor"))
                return Forbid();

            var svc = await _context.Services
                .Include(s => s.Patient).ThenInclude(p => p.User)
                .Include(s => s.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(s => s.Id == serviceId);

            if (svc == null)
                return NotFound();

            // Ensure this service belongs to DoctorVisit
            if (svc.ServiceCategory != "DoctorVisit")
            {
                return Forbid();
            }

            // If DoctorId is set and does not match this doctor, forbid
            var doc = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doc == null) return Forbid();

            if (svc.DoctorId.HasValue && svc.DoctorId != doc.Id)
            {
                return Forbid();
            }

            ServiceData = svc;
            PatientName = svc.Patient.User.FirstName + " " + svc.Patient.User.LastName;
            UpdatedDiagnosis = svc.Diagnosis;
            UpdatedComments = svc.Comments;

            // Other services in the same Visit
            ServicesInSameVisit = await _context.Services
                .Include(x => x.Doctor).ThenInclude(d => d.User)
                .Where(x => x.VisitId == svc.VisitId && x.Id != svc.Id)
                .OrderBy(x => x.ServiceDate)
                .ToListAsync();

            // Images
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

        // Update Diagnosis/Comments
        public async Task<IActionResult> OnPostUpdateDiagnosisAsync(int serviceId)
        {
            var userId = _userManager.GetUserId(User);
            var doc = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doc == null) return Forbid();

            var svc = await _context.Services.FindAsync(serviceId);
            if (svc == null) return NotFound();

            // Check category
            if (svc.ServiceCategory != "DoctorVisit")
            {
                return Forbid();
            }

            if (svc.DoctorId.HasValue && svc.DoctorId != doc.Id)
            {
                return Forbid();
            }

            svc.Diagnosis = UpdatedDiagnosis;
            svc.Comments = UpdatedComments;
            _context.Services.Update(svc);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { serviceId });
        }

        // Finish Visit
        public async Task<IActionResult> OnPostFinishVisitAsync(int serviceId)
        {
            var userId = _userManager.GetUserId(User);
            var doc = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doc == null) return Forbid();

            var svc = await _context.Services.FindAsync(serviceId);
            if (svc == null) return NotFound();

            if (svc.ServiceCategory != "DoctorVisit")
            {
                return Forbid();
            }
            if (svc.DoctorId.HasValue && svc.DoctorId != doc.Id)
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
            return RedirectToPage("/Doctor/Index");
        }

        // Change category on submit
        public async Task<IActionResult> OnPost(int serviceId, string dummy = null)
        {
            // Just refresh
            return await OnGetAsync(serviceId);
        }

        // ReferConfirm
        public async Task<IActionResult> OnPostReferConfirmAsync(int serviceId)
        {
            var userId = _userManager.GetUserId(User);
            var doc = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doc == null) return Forbid();

            var svc = await _context.Services.FindAsync(serviceId);
            if (svc == null) return NotFound();

            if (svc.ServiceCategory != "DoctorVisit")
            {
                return Forbid();
            }
            if (svc.DoctorId.HasValue && svc.DoctorId != doc.Id)
            {
                return Forbid();
            }

            // Mark current service as Done
            svc.ServiceStatus = "Done";
            _context.Services.Update(svc);

            if (string.IsNullOrEmpty(SelectedCategory) || !SelectedServiceDefinitionId.HasValue)
            {
                ModelState.AddModelError("", "Please select Department and Service Definition.");
                await OnGetAsync(serviceId);
                return Page();
            }

            var sd = await _context.ServiceDefinitions.FindAsync(SelectedServiceDefinitionId.Value);
            if (sd == null)
            {
                ModelState.AddModelError("", "Selected ServiceDefinition not found.");
                await OnGetAsync(serviceId);
                return Page();
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
                Comments = $"Referred by Doctor {doc.Id}"
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
            return RedirectToPage("/Doctor/Index");
        }
    }
}
