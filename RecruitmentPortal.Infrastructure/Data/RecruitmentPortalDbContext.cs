using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecruitmentPortal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitmentPortal.Infrastructure.Data
{
    public class RecruitmentPortalDbContext : IdentityDbContext<ApplicationUser>
    {
        
        
        public RecruitmentPortalDbContext(DbContextOptions options) : base(options)
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Data Source=122.170.108.155,9905; Initial Catalog=Recruitment_Portal; User ID=sa; Password=Ask4Apple; Integrated Security=True; Trusted_Connection=False";
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SchedulesUsers>().HasKey(sc => new { sc.scheduleId, sc.UserId });

            ////for candidate relationship with Job application
            //modelBuilder.Entity<Candidate>()
            //.HasMany<JobApplications>(s => s.JobApplications)
            //.WithOne(ad => ad.Candidate)
            //.HasForeignKey<JobApplications>(ad => ad.candidateId);

            //modelBuilder.Entity<Candidate>()
            //.HasMany<JobApplications>(g => g.JobApplications)
            //.WithOne(s => s.Candidate)
            //.HasForeignKey(s => s.CurrentGradeId);
        }
        public DbSet<JobCategory> JobCategory { get; set; }
        public DbSet<JobPost> JobPost { get; set; }
        public DbSet<Degree> Degree { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Candidate> Candidate { get; set; }
        public DbSet<Schedules> Schedules { get; set; }
        public DbSet<SchedulesUsers> SchedulesUsers { get; set; }
        public DbSet<JobApplications> jobApplications { get; set; }
        public DbSet<JobPostCandidate> JobPostCandidate { get; set; }
    }
}
