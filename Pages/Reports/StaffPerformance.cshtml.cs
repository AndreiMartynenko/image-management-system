using HealthcareIMS.Data;
using HealthcareIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareIMS.Pages.Reports
{
    [Authorize(Roles = "Admin,Accountant")]
    public class StaffPerformanceModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StaffPerformanceModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // فیلتر زمانی
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // DTOs
        public List<StaffStat> DoctorStats { get; set; }
        public List<StaffStat> RadStats { get; set; }

        public class StaffStat
        {
            public string DoctorName { get; set; }
            public int CountServices { get; set; }
            public decimal TotalCost { get; set; }
        }

        public async Task OnGetAsync(DateTime? startDate, DateTime? endDate)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;

            var query = _context.Services
                .Include(s => s.Doctor).ThenInclude(d => d.User)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(s => s.ServiceDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                var ed = endDate.Value.AddDays(1).AddSeconds(-1);
                query = query.Where(s => s.ServiceDate <= ed);
            }

            // Doctors
            var docServices = await query
                .Where(s => s.ServiceCategory == "DoctorVisit" && s.DoctorId != null)
                .GroupBy(s => s.DoctorId)
                .Select(g => new
                {
                    DoctorId = g.Key.Value,
                    Count = g.Count(),
                    SumCost = g.Sum(x => x.ServiceCost)
                })
                .ToListAsync();

            DoctorStats = new List<StaffStat>();
            foreach (var grp in docServices)
            {
                var doc = await _context.Doctors
                    .Include(d => d.User)
                    .FirstOrDefaultAsync(d => d.Id == grp.DoctorId);

                if (doc != null)
                {
                    DoctorStats.Add(new StaffStat
                    {
                        DoctorName = doc.User.FirstName + " " + doc.User.LastName,
                        CountServices = grp.Count,
                        TotalCost = grp.SumCost
                    });
                }
            }

            // Radiologists
            var radServices = await query
                .Where(s => s.ServiceCategory == "Radiology" && s.DoctorId != null)
                .GroupBy(s => s.DoctorId)
                .Select(g => new
                {
                    DoctorId = g.Key.Value,
                    Count = g.Count(),
                    SumCost = g.Sum(x => x.ServiceCost)
                })
                .ToListAsync();

            RadStats = new List<StaffStat>();
            foreach (var grp in radServices)
            {
                var doc = await _context.Doctors
                    .Include(d => d.User)
                    .FirstOrDefaultAsync(d => d.Id == grp.DoctorId);

                if (doc != null)
                {
                    RadStats.Add(new StaffStat
                    {
                        DoctorName = doc.User.FirstName + " " + doc.User.LastName,
                        CountServices = grp.Count,
                        TotalCost = grp.SumCost
                    });
                }
            }
        }
    }
}
