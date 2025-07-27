// Data/ApplicationDbContext.cs
using HeathCare.Models;
using HeathCare.Models.HeathCare.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HealthCare.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Career> Careers { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Doctor configuration
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Department)
                .WithMany(d => d.Doctors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Department)
                .WithMany()
                .HasForeignKey(b => b.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Services)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review configuration - simplified
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Doctor)
                .WithMany(d => d.Reviews)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
            // Add to OnModelCreating
            modelBuilder.Entity<JobApplication>()
                .HasOne(j => j.Career)
                .WithMany()
                .HasForeignKey(j => j.CareerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed initial data with ImagePath
            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    Name = "Cardiology",
                    Description = "Heart care specialists",
                    ImagePath = "/uploads/departments/default-cardiology.jpg"
                },
                new Department
                {
                    Id = 2,
                    Name = "Neurology",
                    Description = "Brain and nervous system specialists",
                    ImagePath = "/uploads/departments/default-neurology.jpg"
                },
                new Department
                {
                    Id = 3,
                    Name = "Pediatrics",
                    Description = "Child healthcare specialists",
                    ImagePath = "/uploads/departments/default-pediatrics.jpg"
                }
            );
        }
    }
}