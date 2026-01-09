using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthcareIMS.Pages.Reports
{
    [Authorize(Roles = "Admin,Accountant,Doctor,Radiologist,Reception")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            // فقط صفحه را نمایش می‌دهد
        }
    }
}
