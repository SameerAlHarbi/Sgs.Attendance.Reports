using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sgs.Attendance.Reports.Migrations
{
    public partial class ProcessingCompletedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "ProcessingRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "ProcessingRequests");
        }
    }
}
