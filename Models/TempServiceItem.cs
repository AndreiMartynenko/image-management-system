namespace HealthcareIMS.ViewModels
{
    public class TempServiceItem
    {
        public int ServiceDefinitionId { get; set; }
        public string ServiceName { get; set; }
        public decimal Cost { get; set; }

        public int? DoctorId { get; set; }
        public string DoctorName { get; set; }

        // اگر خواستید چیزهای دیگر (Category و …)
        public string Category { get; set; }
    }
}
