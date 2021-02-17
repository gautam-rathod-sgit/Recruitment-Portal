﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecruitmentPortal.Infrastructure.Data;

namespace RecruitmentPortal.WebApp.Migrations
{
    [DbContext(typeof(RecruitmentPortalDbContext))]
    [Migration("20210216113532_PK added 1")]
    partial class PKadded1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("position")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("skype_id")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.Candidate", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("degree")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("emailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("experience")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("hsc_perc")
                        .HasColumnType("real");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("phone")
                        .HasColumnType("float");

                    b.Property<string>("resume")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("ssc_perc")
                        .HasColumnType("real");

                    b.HasKey("ID");

                    b.ToTable("Candidate");
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.Degree", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("degree_name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Degree");
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.Department", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DegreeId")
                        .HasColumnType("int");

                    b.Property<string>("dept_name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("DegreeId");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.JobApplications", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("candidateId")
                        .HasColumnType("int");

                    b.Property<DateTime>("joining_date")
                        .HasColumnType("datetime2");

                    b.Property<bool>("notified")
                        .HasColumnType("bit");

                    b.Property<int>("round")
                        .HasColumnType("int");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("candidateId");

                    b.ToTable("jobApplications");
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.JobCategory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("job_category_name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("JobCategory");
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.JobPost", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("JobCategoryId")
                        .HasColumnType("int");

                    b.Property<string>("eligibility_criteria")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("experience")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("job_role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("job_title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("job_type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("skills")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("vacancy")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("JobCategoryId");

                    b.ToTable("JobPost");
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.JobPostCandidate", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("candidate_Id")
                        .HasColumnType("int");

                    b.Property<int>("job_Id")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("candidate_Id");

                    b.HasIndex("job_Id");

                    b.ToTable("JobPostCandidate");
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.Schedules", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("applying_through")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("candidateId")
                        .HasColumnType("int");

                    b.Property<string>("candidate_name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("datetime")
                        .HasColumnType("datetime2");

                    b.Property<string>("location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("mode_of_interview")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("position")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("rating")
                        .HasColumnType("real");

                    b.Property<string>("remark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("round")
                        .HasColumnType("int");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("candidateId");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.SchedulesUsers", b =>
                {
                    b.Property<int>("scheduleId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ID")
                        .HasColumnType("int");

                    b.HasKey("scheduleId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("SchedulesUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("RecruitmentPortal.Core.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("RecruitmentPortal.Core.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RecruitmentPortal.Core.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("RecruitmentPortal.Core.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.Department", b =>
                {
                    b.HasOne("RecruitmentPortal.Core.Entities.Degree", "Degree")
                        .WithMany("Departments")
                        .HasForeignKey("DegreeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.JobApplications", b =>
                {
                    b.HasOne("RecruitmentPortal.Core.Entities.Candidate", null)
                        .WithMany("JobApplications")
                        .HasForeignKey("candidateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.JobPost", b =>
                {
                    b.HasOne("RecruitmentPortal.Core.Entities.JobCategory", "JobCategory")
                        .WithMany("JobPosts")
                        .HasForeignKey("JobCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.JobPostCandidate", b =>
                {
                    b.HasOne("RecruitmentPortal.Core.Entities.Candidate", "Candidate")
                        .WithMany("JobPostCandidate")
                        .HasForeignKey("candidate_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RecruitmentPortal.Core.Entities.JobPost", "JobPost")
                        .WithMany("JobPostCandidate")
                        .HasForeignKey("job_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.Schedules", b =>
                {
                    b.HasOne("RecruitmentPortal.Core.Entities.Candidate", "Candidate")
                        .WithMany("Schedules")
                        .HasForeignKey("candidateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RecruitmentPortal.Core.Entities.SchedulesUsers", b =>
                {
                    b.HasOne("RecruitmentPortal.Core.Entities.ApplicationUser", "ApplicationUser")
                        .WithMany("SchedulesUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RecruitmentPortal.Core.Entities.Schedules", "Schedules")
                        .WithMany("SchedulesUsers")
                        .HasForeignKey("scheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
