using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT_Daily_Check.Migrations
{
    public partial class addedimageuploadfunctionality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageOneName",
                table: "DailyChecks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageTwoName",
                table: "DailyChecks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageOneName",
                table: "DailyChecks");

            migrationBuilder.DropColumn(
                name: "ImageTwoName",
                table: "DailyChecks");
        }
    }
}
