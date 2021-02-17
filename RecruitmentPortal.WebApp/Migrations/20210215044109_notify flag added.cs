using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentPortal.WebApp.Migrations
{
    public partial class notifyflagadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "notified",
                table: "jobApplications",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "notified",
                table: "jobApplications");
        }
    }
}
