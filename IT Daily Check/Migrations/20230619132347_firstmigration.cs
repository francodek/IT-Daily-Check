using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT_Daily_Check.Migrations
{
    public partial class firstmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CCTVs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CCTVs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyChecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date_Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_By = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyChecks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "internetServiceProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_internetServiceProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CCTVchecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Results = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reasons = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DailyChecksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CCTVchecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CCTVchecks_DailyChecks_DailyChecksId",
                        column: x => x.DailyChecksId,
                        principalTable: "DailyChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceServicechecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DailyChecksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceServicechecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceServicechecks_DailyChecks_DailyChecksId",
                        column: x => x.DailyChecksId,
                        principalTable: "DailyChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "internetServiceSpeedchecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ISP_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DownloadSpeed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UploadSpeed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailyChecksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_internetServiceSpeedchecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_internetServiceSpeedchecks_DailyChecks_DailyChecksId",
                        column: x => x.DailyChecksId,
                        principalTable: "DailyChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CCTVchecks_DailyChecksId",
                table: "CCTVchecks",
                column: "DailyChecksId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceServicechecks_DailyChecksId",
                table: "DeviceServicechecks",
                column: "DailyChecksId");

            migrationBuilder.CreateIndex(
                name: "IX_internetServiceSpeedchecks_DailyChecksId",
                table: "internetServiceSpeedchecks",
                column: "DailyChecksId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CCTVchecks");

            migrationBuilder.DropTable(
                name: "CCTVs");

            migrationBuilder.DropTable(
                name: "DeviceServicechecks");

            migrationBuilder.DropTable(
                name: "DeviceServices");

            migrationBuilder.DropTable(
                name: "internetServiceProviders");

            migrationBuilder.DropTable(
                name: "internetServiceSpeedchecks");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "DailyChecks");
        }
    }
}
