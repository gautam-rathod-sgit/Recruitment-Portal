using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Repository;
using RecruitmentPortal.Core.Repository.Base;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Repository;
using RecruitmentPortal.Infrastructure.Repository.Base;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.Services;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //Adding service for DbContext
            //use Real Database
            services.AddDbContext<RecruitmentPortalDbContext>(c =>
            c.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsAssembly("RecruitmentPortal.WebApp")));

            // Add ASP.NET Identity support
            services.AddIdentity<ApplicationUser, IdentityRole>(
            opts =>
            {
                opts.Password.RequireDigit = true;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<RecruitmentPortalDbContext>();
            //services of upper generics
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); //for generic

            //Service of upper layer specifics

            services.AddScoped<IJobCategoryRepository, JobCategoryRepository>(); //for JobCategory model
            services.AddScoped<IJobPostRepository, JobPostRepository>(); //for JobPost model
            services.AddScoped<IDegreeRepository, DegreeRepository>(); //for Degree model
            services.AddScoped<IDepartmentRepository, DepartmentRepository>(); //for Department model
            services.AddScoped<ICandidateRepository, CandidateRepository>(); //for Candidate model
            services.AddScoped<ISchedulesRepository, SchedulesRepository>(); //for Schedules model
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>(); //for Schedules model
            services.AddScoped<IJobPostCandidateRepository, JobPostCandidateRepository>(); //for JobPostCandidate model
            services.AddScoped<ISchedulesUsersRepository, SchedulesUsersRepository>(); //for SchedulesUsers model


            //Service of my layer :

            services.AddScoped<IJobCategoryPage, JobCategoryPageService>(); //for JobCategoryViewmodel
            services.AddScoped<IJobPostPage, JobPostPageService>(); //for JobPostViewmodel
            services.AddScoped<IDegreePage, DegreePageService>(); //for DegreeViewmodel
            services.AddScoped<IDepartmentPage, DepartmentPageService>(); //for DepartmentViewmodel
            services.AddScoped<ICandidatePage, CandidatePageService>(); //for CandidateViewmodel
            services.AddScoped<ISchedulesPage, SchedulesPageService>(); //for SchedulesViewmodel
            services.AddScoped<IJobApplicationPage, JobApplicationPageService>(); //for SchedulesViewmodel
            services.AddScoped<IJobPostCandidatePage, JobPostCandidatePageService>(); //for JobPostCandidateViewModel
            services.AddScoped<ISchedulesUsersPage, SchedulesUsersPageService>(); //for SchedulesUsersViewModel

            //email
            services.Configure<SMTPConfigModel>(Configuration.GetSection("SMTPConfig"));
            services.AddTransient<IEmailService, EmailService>();

            //services of automapper
            services.AddAutoMapper(typeof(Startup));
            services.AddControllersWithViews();
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}