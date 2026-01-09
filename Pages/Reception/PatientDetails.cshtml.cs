using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HealthcareIMS.Pages.Reception
{
    [Authorize(Roles = "Reception")]
    public class PatientDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PatientDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Patient PatientData { get; set; }

        [BindProperty]
        public User UserData { get; set; } // برای دسترسی به اطلاعات User

        public async Task<IActionResult> OnGetAsync(int id)
        {
            PatientData = await _context.Patients
                .Include(p => p.User) // لود کردن User مرتبط
                .FirstOrDefaultAsync(p => p.Id == id);

            if (PatientData == null) return NotFound();

            // اطلاعات User را برای نمایش در فرم مقداردهی کنید
            UserData = PatientData.User;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // پیدا کردن بیمار در پایگاه داده
            var patient = await _context.Patients
                .Include(p => p.User) // لود User مرتبط
                .FirstOrDefaultAsync(p => p.Id == PatientData.Id);

            if (patient == null)
                return NotFound();

            if (patient.User != null)
            {
                // به‌روزرسانی اطلاعات User مرتبط با Patient
                patient.User.FirstName = UserData.FirstName;
                patient.User.LastName = UserData.LastName;
                patient.User.DateOfBirth = UserData.DateOfBirth;
                patient.User.Gender = UserData.Gender;
                patient.User.PhoneNumber = UserData.PhoneNumber;
                patient.User.Address = UserData.Address;
            }

            // ذخیره تغییرات
            await _context.SaveChangesAsync();

            return RedirectToPage("/Reception/Index");
        }
    }
}
