using Microsoft.EntityFrameworkCore.Migrations;

namespace Sgs.Attendance.Reports.Migrations
{
    public partial class fixShiftRepeat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ShiftRepeat",
                table: "WorkShifts",
                nullable: false,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "ShiftRepeat",
                table: "WorkShifts",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
