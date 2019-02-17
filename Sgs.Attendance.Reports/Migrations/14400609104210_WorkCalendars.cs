using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sgs.Attendance.Reports.Migrations
{
    public partial class WorkCalendars : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkCalendars",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ContractWorkTime = table.Column<int>(nullable: false),
                    IsVacationCalendar = table.Column<bool>(nullable: false),
                    VacationDescription = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkCalendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkShifts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShiftOrder = table.Column<int>(nullable: false),
                    ShiftRepeat = table.Column<bool>(nullable: false),
                    ShiftStart = table.Column<double>(nullable: true),
                    ShiftEnd = table.Column<double>(nullable: true),
                    ShiftStartInRamadan = table.Column<double>(nullable: true),
                    ShiftEndInRamadan = table.Column<double>(nullable: true),
                    IsDayOff = table.Column<bool>(nullable: false),
                    DayOffDescription = table.Column<string>(nullable: true),
                    IsDayOffInRamadan = table.Column<bool>(nullable: true),
                    DayOffDescriptionInRamadan = table.Column<string>(nullable: true),
                    WorkCalendarId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkShifts_WorkCalendars_WorkCalendarId",
                        column: x => x.WorkCalendarId,
                        principalTable: "WorkCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkShifts_WorkCalendarId",
                table: "WorkShifts",
                column: "WorkCalendarId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkShifts");

            migrationBuilder.DropTable(
                name: "WorkCalendars");
        }
    }
}
