using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentPortal.WebApp.Migrations
{
    public partial class joiningdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "joining_date",
                table: "jobApplications",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "joining_date",
                table: "jobApplications");
        }
    }
}
