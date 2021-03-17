using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using RecruitmentPortal.Core.Entities;
using RecruitmentPortal.Core.Repository;
using RecruitmentPortal.Core.Repository.Base;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Helpers;
using RecruitmentPortal.Infrastructure.Repository;
using RecruitmentPortal.Infrastructure.Repository.Base;
using RecruitmentPortal.WebApp.Helpers;
using RecruitmentPortal.WebApp.Interfaces;
using RecruitmentPortal.WebApp.Mapper;
using RecruitmentPortal.WebApp.Security;
using RecruitmentPortal.WebApp.Services;
using RecruitmentPortal.WebApp.ViewModels;
using System;
using System.Linq;

namespace RecruitmentPortal.WebApp
{
    public class Startup
    {
        private MapperConfiguration _mapperConfiguration { get; }
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _mapperConfiguration = new MapperConfiguration(cfg => { cfg.AddProfile(new MapperProfile()); });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => { options.LoginPath = "/Login/Login"; });
            services.AddControllersWithViews();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession();

            //Adding service for DbContext
            //use Real Database
            services.AddDbContext<RecruitmentPortalDbContext>(c =>
            c.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsAssembly("RecruitmentPortal.WebApp")));
            CommonHelper.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
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
            .AddEntityFrameworkStores<RecruitmentPortalDbContext>()
            .AddDefaultTokenProviders();  //for token generation

            //for timspan of token
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromHours(2));

            //email
            services.Configure<SMTPConfigModel>(Configuration.GetSection("SMTPConfig"));
            services.AddTransient<IEmailService, EmailService>();
            services.AddSingleton<DataProtectionPurposeStrings>();

            //services of automapper
            services.AddAutoMapper(typeof(Startup));

            //Url Encryption 
            services.AddDataProtection();
            services.AddMvc(options => options.EnableEndpointRouting = false);
                //.AddJsonOptions(x => { x.SerializerSettings.ContractResolver = new DefaultContractResolver(); });
            services.AddSingleton(sp => _mapperConfiguration.CreateMapper());
            RegisterServices(services);
        }

        private void RegisterServices(IServiceCollection services)
        {
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestLocalization();
            app.UseAuthorization();
            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseSession();

            HttpHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            //app.UseRouting();
            

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller=Home}/{action=Index}/{id?}");
            //    endpoints.MapRazorPages();
            //});

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}