using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentPortal.WebApp.Migrations
{
    public partial class testerchanges2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "applying_through",
                table: "Candidate",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "applying_through",
                table: "Candidate");
        }
    }
}
