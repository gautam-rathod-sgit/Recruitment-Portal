using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentPortal.WebApp.Migrations
{
    public partial class testerchanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "JobPost",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "JobCategory",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "accept_date",
                table: "jobApplications",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "commitment_mode",
                table: "jobApplications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "offered_ctc",
                table: "jobApplications",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "rejection_date",
                table: "jobApplications",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "rejection_reason",
                table: "jobApplications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "remarks",
                table: "jobApplications",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "jobApplications",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Department",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Degree",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "apply_date",
                table: "Candidate",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "current_ctc",
                table: "Candidate",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "dob",
                table: "Candidate",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "expected_ctc",
                table: "Candidate",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notice_period",
                table: "Candidate",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "JobPost");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "JobCategory");

            migrationBuilder.DropColumn(
                name: "accept_date",
                table: "jobApplications");

            migrationBuilder.DropColumn(
                name: "commitment_mode",
                table: "jobApplications");

            migrationBuilder.DropColumn(
                name: "offered_ctc",
                table: "jobApplications");

            migrationBuilder.DropColumn(
                name: "rejection_date",
                table: "jobApplications");

            migrationBuilder.DropColumn(
                name: "rejection_reason",
                table: "jobApplications");

            migrationBuilder.DropColumn(
                name: "remarks",
                table: "jobApplications");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "jobApplications");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Degree");

            migrationBuilder.DropColumn(
                name: "apply_date",
                table: "Candidate");

            migrationBuilder.DropColumn(
                name: "current_ctc",
                table: "Candidate");

            migrationBuilder.DropColumn(
                name: "dob",
                table: "Candidate");

            migrationBuilder.DropColumn(
                name: "expected_ctc",
                table: "Candidate");

            migrationBuilder.DropColumn(
                name: "notice_period",
                table: "Candidate");
        }
    }
}
