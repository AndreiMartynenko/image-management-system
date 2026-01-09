using Microsoft.EntityFrameworkCore;
using HealthcareIMS.Models;
using System;

namespace HealthcareIMS.Data
{
    public class HealthcareContext : DbContext
    {
        public HealthcareContext(DbContextOptions<HealthcareContext> options)
            : base(options)
        {
        }

        // DbSetها:
        public DbSet<Person> Persons { get; set; }        // کلاس پدر
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Radiologist> Radiologists { get; set; }
        public DbSet<Patient> Patients { get; set; }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<MedicalImage> MedicalImages { get; set; }

        // اگر قصد استفاده از TPH دارید، نیاز به پیکربندی اضافی دارید:
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     // پیکربندی TPH (در صورت نیاز)
        //     // modelBuilder.Entity<Person>()
        //     //    .HasDiscriminator<string>("PersonType")
        //     //    .HasValue<Person>("BasePerson")
        //     //    .HasValue<Doctor>("Doctor")
        //     //    .HasValue<Radiologist>("Radiologist")
        //     //    .HasValue<Patient>("Patient");
        // }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // رفع هشدار مربوط به decimal(18,2) 
            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Invoice>()
                .Property(i => i.InvoiceDate)
                .HasColumnType("datetime2"); // صریحاً نوع DateTime را برای SQL Server مشخص کنید

            // جلوگیری از چند مسیر حذف آبشاری برای جدول Appointments
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Radiologist)
                .WithMany()
                .HasForeignKey(a => a.RadiologistId)
                .OnDelete(DeleteBehavior.Restrict);

            // جلوگیری از چند مسیر حذف آبشاری برای جدول MedicalImages
            modelBuilder.Entity<MedicalImage>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalImages)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalImage>()
                .HasOne(m => m.Radiologist)
                .WithMany()
                .HasForeignKey(m => m.RadiologistId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }


    }
}
