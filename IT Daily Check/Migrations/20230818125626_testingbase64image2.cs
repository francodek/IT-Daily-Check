using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT_Daily_Check.Migrations
{
    public partial class testingbase64image2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageTwoBase64",
                table: "DailyChecks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageTwoBase64",
                table: "DailyChecks");
        }
    }
}
