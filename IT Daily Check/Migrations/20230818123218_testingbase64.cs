using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT_Daily_Check.Migrations
{
    public partial class testingbase64 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageOneBase64",
                table: "DailyChecks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageOneBase64",
                table: "DailyChecks");
        }
    }
}
