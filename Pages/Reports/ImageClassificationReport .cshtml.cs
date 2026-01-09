using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Reports
{
    [Authorize(Roles = "Admin,Radiologist,Doctor,Accountant")]
    public class ImageClassificationReportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ImageClassificationReportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string FilterImagingType { get; set; } // MRI, CT, ...
        [BindProperty(SupportsGet = true)]
        public string FilterDiseaseType { get; set; } // e.g. "Lung Cancer"

        public List<Imaging> ImagingList { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Imagings.AsQueryable();

            if (!string.IsNullOrEmpty(FilterImagingType))
            {
                query = query.Where(i => i.ImagingType.Contains(FilterImagingType));
            }
            if (!string.IsNullOrEmpty(FilterDiseaseType))
            {
                query = query.Where(i => i.DiseaseType.Contains(FilterDiseaseType));
            }

            ImagingList = await query
                .OrderByDescending(i => i.CaptureDate)
                .Take(500)
                .ToListAsync();
        }
    }
}
