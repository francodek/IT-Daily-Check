using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT_Daily_Check.Migrations
{
    public partial class userphoneposition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_internetServiceSpeedchecks_DailyChecks_DailyChecksId",
                table: "internetServiceSpeedchecks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_internetServiceSpeedchecks",
                table: "internetServiceSpeedchecks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_internetServiceProviders",
                table: "internetServiceProviders");

            migrationBuilder.RenameTable(
                name: "internetServiceSpeedchecks",
                newName: "InternetServiceSpeedchecks");

            migrationBuilder.RenameTable(
                name: "internetServiceProviders",
                newName: "InternetServiceProviders");

            migrationBuilder.RenameIndex(
                name: "IX_internetServiceSpeedchecks_DailyChecksId",
                table: "InternetServiceSpeedchecks",
                newName: "IX_InternetServiceSpeedchecks_DailyChecksId");

            migrationBuilder.AddColumn<string>(
                name: "UserPhoneNumber",
                table: "DailyChecks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserPosition",
                table: "DailyChecks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InternetServiceSpeedchecks",
                table: "InternetServiceSpeedchecks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InternetServiceProviders",
                table: "InternetServiceProviders",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InternetServiceSpeedchecks_DailyChecks_DailyChecksId",
                table: "InternetServiceSpeedchecks",
                column: "DailyChecksId",
                principalTable: "DailyChecks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InternetServiceSpeedchecks_DailyChecks_DailyChecksId",
                table: "InternetServiceSpeedchecks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InternetServiceSpeedchecks",
                table: "InternetServiceSpeedchecks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InternetServiceProviders",
                table: "InternetServiceProviders");

            migrationBuilder.DropColumn(
                name: "UserPhoneNumber",
                table: "DailyChecks");

            migrationBuilder.DropColumn(
                name: "UserPosition",
                table: "DailyChecks");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "InternetServiceSpeedchecks",
                newName: "internetServiceSpeedchecks");

            migrationBuilder.RenameTable(
                name: "InternetServiceProviders",
                newName: "internetServiceProviders");

            migrationBuilder.RenameIndex(
                name: "IX_InternetServiceSpeedchecks_DailyChecksId",
                table: "internetServiceSpeedchecks",
                newName: "IX_internetServiceSpeedchecks_DailyChecksId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_internetServiceSpeedchecks",
                table: "internetServiceSpeedchecks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_internetServiceProviders",
                table: "internetServiceProviders",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_internetServiceSpeedchecks_DailyChecks_DailyChecksId",
                table: "internetServiceSpeedchecks",
                column: "DailyChecksId",
                principalTable: "DailyChecks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
