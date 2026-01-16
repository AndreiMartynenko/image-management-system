using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Reception
{
    [Authorize(Roles = "Reception")]
    public class ManageVisitServicesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public ManageVisitServicesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string SearchTerm { get; set; }

        [BindProperty]
        public int? PatientId { get; set; }

        [BindProperty]
        public int? OpenVisitId { get; set; }

        // مرحله اول: انتخاب Category
        [BindProperty]
        public string SelectedCategory { get; set; }

        // مرحله دوم: انتخاب ServiceDefinition (در همان دسته)
        [BindProperty]
        public int? SelectedServiceDefinitionId { get; set; }

        [BindProperty]
        public int SelectedDoctorId { get; set; }

        public List<Patient> FoundPatients { get; set; }

        // لیست دسته‌های سرویس (Categoryها)
        public List<string> AllCategories { get; set; }

        // لیست ServiceDefinitionهای همین دسته
        public List<ServiceDefinition> FilteredServiceDefinitions { get; set; }

        // دکترها
        public List<HealthcareIMS.Models.Doctor> Doctors { get; set; }

        public List<Service> ServicesOfVisit { get; set; }

        // نمایش بیمار انتخابی
        public Patient SelectedPatient
        {
            get
            {
                if (PatientId.HasValue)
                {
                    return _context.Patients
                        .Include(p => p.User)
                        .FirstOrDefault(p => p.Id == PatientId.Value);
                }
                return null;
            }
        }

        public async Task OnGetAsync()
        {
            await LoadDropdowns();

            ServicesOfVisit = new List<Service>();
            if (OpenVisitId.HasValue)
            {
                ServicesOfVisit = await _context.Services
                    .Where(s => s.VisitId == OpenVisitId.Value)
                    .Include(s => s.Doctor).ThenInclude(d => d.User)
                    .OrderBy(s => s.ServiceDate) // مرتب بر اساس تاریخ
                    .ToListAsync();
            }
        }

        private async Task LoadDropdowns()
        {
            // دکترها
            Doctors = await _context.Doctors
                .Include(d => d.User)
                .ToListAsync();

            // همه ServiceDefinitions
            var defs = await _context.ServiceDefinitions
                .Where(sd => sd.IsActive)
                .ToListAsync();

            // مرحله اول: لیست Category
            AllCategories = defs
                .Select(sd => sd.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            // اگر SelectedCategory پر باشد، لیست ServiceDefinitionهای همان دسته را می‌گیریم
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

        // SearchPatient
        public async Task<IActionResult> OnPostSearchPatientAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                ModelState.AddModelError("", "Please enter search term.");
                await LoadDropdowns();
                return Page();
            }

            FoundPatients = await _context.Patients
                .Include(p => p.User)
                .Where(p => (p.User.FirstName + " " + p.User.LastName).Contains(SearchTerm)
                         || p.Id.ToString() == SearchTerm)
                .Take(50)
                .ToListAsync();

            await LoadDropdowns();
            return Page();
        }

        // SelectPatient
        public async Task<IActionResult> OnPostSelectPatientAsync(int patientId)
        {
            var pat = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (pat == null)
            {
                ModelState.AddModelError("", "Patient not found.");
                await LoadDropdowns();
                return Page();
            }

            PatientId = pat.Id;

            // ویزیت باز؟
            var visit = await _context.Visits
                .Where(v => v.PatientId == pat.Id && v.VisitStatus == "Open")
                .FirstOrDefaultAsync();

            if (visit == null)
            {
                // بساز
                visit = new Visit
                {
                    PatientId = pat.Id,
                    VisitDate = DateTime.Now,
                    VisitStatus = "Open"
                };
                _context.Visits.Add(visit);
                await _context.SaveChangesAsync();
            }

            OpenVisitId = visit.Id;

            ServicesOfVisit = await _context.Services
                .Where(s => s.VisitId == visit.Id)
                .Include(s => s.Doctor).ThenInclude(d => d.User)
                .OrderBy(s => s.ServiceDate)
                .ToListAsync();

            await LoadDropdowns();
            return Page();
        }

        // AddService
        public async Task<IActionResult> OnPostAddServiceAsync()
        {
            // اطمینان از اینکه Patient و Visit انتخاب شده
            if (!PatientId.HasValue || !OpenVisitId.HasValue)
            {
                ModelState.AddModelError("", "No patient or visit is selected!");
                await LoadDropdowns();
                return Page();
            }

            var visit = await _context.Visits.FindAsync(OpenVisitId.Value);
            if (visit == null)
            {
                ModelState.AddModelError("", "Visit not found in DB.");
                await LoadDropdowns();
                return Page();
            }

            if (!SelectedServiceDefinitionId.HasValue)
            {
                ModelState.AddModelError("", "No service definition selected!");
                await LoadDropdowns();
                return Page();
            }

            var sd = await _context.ServiceDefinitions.FindAsync(SelectedServiceDefinitionId.Value);
            if (sd == null)
            {
                ModelState.AddModelError("", "ServiceDefinition not found.");
                await LoadDropdowns();
                return Page();
            }

            // ساخت سرویس
            var newService = new Service
            {
                VisitId = visit.Id,
                PatientId = visit.PatientId,
                ServiceName = sd.Name,
                ServiceCategory = sd.Category,
                ServiceCost = sd.DefaultCost,
                ServiceDate = DateTime.Now,
                ServiceStatus = "Pending",
                Comments = "Created by Reception"
            };

            if (SelectedDoctorId == 0)
            {
                newService.DoctorId = null;
            }
            else
            {
                var doc = await _context.Doctors.FindAsync(SelectedDoctorId);
                if (doc == null)
                {
                    ModelState.AddModelError("", "Doctor not found.");
                    await LoadDropdowns();
                    return Page();
                }
                newService.DoctorId = doc.Id;
            }

            _context.Services.Add(newService);
            await _context.SaveChangesAsync();

            // اگر می‌خواهید Appointment هم بسازید
            var appt = new Appointment
            {
                PatientId = visit.PatientId,
                DoctorId = newService.DoctorId,
                AppointmentDate = DateTime.Now,
                Status = "Pending"
            };
            _context.Appointments.Add(appt);
            await _context.SaveChangesAsync();

            // به‌روزرسانی TotalCost در Visit
            if (visit.TotalCost == null) visit.TotalCost = 0;
            visit.TotalCost += sd.DefaultCost;
            _context.Visits.Update(visit);
            await _context.SaveChangesAsync();

            // بروزرسانی یا ایجاد Invoice
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.VisitId == visit.Id);
            if (invoice == null)
            {
                invoice = new Invoice
                {
                    VisitId = visit.Id,
                    TotalAmount = 0,
                    PaymentStatus = "Unpaid",
                    IssuedDate = DateTime.Now
                };
                _context.Invoices.Add(invoice);
            }
            invoice.TotalAmount += sd.DefaultCost;
            await _context.SaveChangesAsync();

            // لیست سرویس‌ها
            ServicesOfVisit = await _context.Services
                .Where(s => s.VisitId == visit.Id)
                .Include(s => s.Doctor).ThenInclude(d => d.User)
                .OrderBy(s => s.ServiceDate)
                .ToListAsync();

            await LoadDropdowns();

            // مقدار dropdown را ریست کنیم
            SelectedCategory = null;
            SelectedServiceDefinitionId = null;

            return Page();
        }

        // RemoveService
        public async Task<IActionResult> OnPostRemoveServiceAsync(int serviceId)
        {
            if (!OpenVisitId.HasValue)
            {
                ModelState.AddModelError("", "No Visit selected!");
                await LoadDropdowns();
                return Page();
            }

            var svc = await _context.Services.FindAsync(serviceId);
            if (svc == null)
            {
                ModelState.AddModelError("", "Service not found in DB.");
                await LoadDropdowns();
                return Page();
            }

            _context.Services.Remove(svc);

            // حذف Appointment
            var appt = await _context.Appointments
                .Where(a => a.PatientId == svc.PatientId && a.DoctorId == svc.DoctorId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefaultAsync();
            if (appt != null)
            {
                _context.Appointments.Remove(appt);
            }

            var visit = await _context.Visits.FindAsync(svc.VisitId);
            if (visit != null && visit.TotalCost > 0)
            {
                visit.TotalCost -= svc.ServiceCost;
                if (visit.TotalCost < 0) visit.TotalCost = 0;
                _context.Visits.Update(visit);
            }

            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.VisitId == svc.VisitId);
            if (invoice != null)
            {
                invoice.TotalAmount -= svc.ServiceCost;
                if (invoice.TotalAmount <= 0)
                {
                    _context.Invoices.Remove(invoice);
                }
                else
                {
                    _context.Invoices.Update(invoice);
                }
            }

            await _context.SaveChangesAsync();

            ServicesOfVisit = await _context.Services
                .Where(s => s.VisitId == svc.VisitId)
                .Include(s => s.Doctor).ThenInclude(d => d.User)
                .OrderBy(s => s.ServiceDate)
                .ToListAsync();

            await LoadDropdowns();
            return Page();
        }
    }
}
