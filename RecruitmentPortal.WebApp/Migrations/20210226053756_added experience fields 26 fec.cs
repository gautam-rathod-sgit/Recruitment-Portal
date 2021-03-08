using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentPortal.WebApp.Migrations
{
    public partial class addedexperiencefields26fec : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "expected_ctc",
                table: "Candidate",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "relevent_experience",
                table: "Candidate",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "total_experience",
                table: "Candidate",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "relevent_experience",
                table: "Candidate");

            migrationBuilder.DropColumn(
                name: "total_experience",
                table: "Candidate");

            migrationBuilder.AlterColumn<string>(
                name: "expected_ctc",
                table: "Candidate",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(double));
        }
    }
}
