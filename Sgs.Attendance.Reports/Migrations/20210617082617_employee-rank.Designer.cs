﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sgs.Attendance.Reports.Data;

namespace Sgs.Attendance.Reports.Migrations
{
    [DbContext(typeof(AttendanceReportsDb))]
    [Migration("20210617082617_employee-rank")]
    partial class employeerank
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Sgs.Attendance.Reports.Models.AbsentNotification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("AbsentDate");

                    b.Property<string>("ByEmployeeId");

                    b.Property<string>("ByEmployeeName");

                    b.Property<int>("EmployeeId");

                    b.Property<DateTime>("SendDate");

                    b.HasKey("Id");

                    b.ToTable("AbsentsNotifications");
                });

            modelBuilder.Entity("Sgs.Attendance.Reports.Models.EmployeeCalendar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AttendanceProof");

                    b.Property<int>("ContractWorkTime");

                    b.Property<int>("EmployeeId");

                    b.Property<DateTime?>("EndDate");

                    b.Property<string>("Note");

                    b.Property<DateTime>("StartDate");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("EmployeesCalendars");
                });

            modelBuilder.Entity("Sgs.Attendance.Reports.Models.EmployeeDayReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("AbsentNotified");

                    b.Property<string>("AbsentNotifiedByEmployeeId");

                    b.Property<string>("AbsentNotifiedByEmployeeName");

                    b.Property<DateTime?>("AbsentNotifiedDate");

                    b.Property<DateTime?>("ActualCheckInDateTime");

                    b.Property<DateTime?>("ActualCheckOutDateTime");

                    b.Property<TimeSpan?>("ActualTotalDurationTime");

                    b.Property<TimeSpan?>("ActualWorkDurationTime");

                    b.Property<int>("AttendanceProof");

                    b.Property<DateTime?>("CheckInDateTime");

                    b.Property<bool>("CheckInExcuse");

                    b.Property<double>("CheckInExcuseHours");

                    b.Property<TimeSpan?>("CheckInLateDurationTime");

                    b.Property<DateTime?>("CheckOutDateTime");

                    b.Property<TimeSpan?>("CheckOutEarlyDurationTime");

                    b.Property<bool>("CheckOutExcuse");

                    b.Property<double>("CheckOutExcuseHours");

                    b.Property<DateTime?>("ContractCheckInDateTime");

                    b.Property<DateTime?>("ContractCheckOutDateTime");

                    b.Property<TimeSpan?>("ContractWorkDurationTime");

                    b.Property<DateTime>("DayDate");

                    b.Property<DateTime?>("DelegationRequestDate");

                    b.Property<int>("DepartmentId");

                    b.Property<string>("DepartmentName");

                    b.Property<int>("EmployeeId");

                    b.Property<string>("EmployeeName");

                    b.Property<string>("EmployeeRank");

                    b.Property<bool>("IsAbsentEmployee");

                    b.Property<bool>("IsDelegationRequest");

                    b.Property<bool>("IsVacation");

                    b.Property<bool>("IsVacationRequest");

                    b.Property<DateTime>("ProcessingDate");

                    b.Property<string>("VacationName");

                    b.Property<DateTime?>("VacationRegisterDate");

                    b.Property<DateTime?>("VacationRequestDate");

                    b.Property<TimeSpan?>("WasteDurationTime");

                    b.Property<TimeSpan?>("WorkDurationTime");

                    b.Property<TimeSpan?>("WorkTotalDurationTime");

                    b.HasKey("Id");

                    b.ToTable("EmployeeDaysReports");
                });

            modelBuilder.Entity("Sgs.Attendance.Reports.Models.EmployeeExcuse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EmployeeId");

                    b.Property<DateTime>("ExcueseDate");

                    b.Property<double?>("ExcuseHours");

                    b.Property<int>("ExcuseType");

                    b.Property<string>("Note");

                    b.Property<DateTime>("RegisterDate");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("EmployeesExcuses");
                });

            modelBuilder.Entity("Sgs.Attendance.Reports.Models.ProcessingRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Completed");

                    b.Property<DateTime?>("CompletedDate");

                    b.Property<string>("Employees");

                    b.Property<DateTime>("FromDate");

                    b.Property<DateTime>("RequestDate");

                    b.Property<DateTime>("ToDate");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("ProcessingRequests");
                });

            modelBuilder.Entity("Sgs.Attendance.Reports.Models.RequestReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ByUserId");

                    b.Property<string>("ByUserName");

                    b.Property<string>("FilterText");

                    b.Property<DateTime>("FromDate");

                    b.Property<string>("ReportType");

                    b.Property<DateTime>("RequestDate");

                    b.Property<DateTime>("ToDate");

                    b.HasKey("Id");

                    b.ToTable("RequestsReports");
                });

            modelBuilder.Entity("Sgs.Attendance.Reports.Models.WorkCalendar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ContractWorkTime");

                    b.Property<DateTime?>("EndDate");

                    b.Property<bool>("IsVacationCalendar");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Note");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.ToTable("WorkCalendars");
                });

            modelBuilder.Entity("Sgs.Attendance.Reports.Models.WorkShift", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DayOffDescription");

                    b.Property<string>("DayOffDescriptionInRamadan");

                    b.Property<bool>("IsDayOff");

                    b.Property<bool?>("IsDayOffInRamadan");

                    b.Property<double?>("ShiftEnd");

                    b.Property<double?>("ShiftEndInRamadan");

                    b.Property<int>("ShiftOrder");

                    b.Property<int>("ShiftRepeat");

                    b.Property<double?>("ShiftStart");

                    b.Property<double?>("ShiftStartInRamadan");

                    b.Property<int>("WorkCalendarId");

                    b.HasKey("Id");

                    b.HasIndex("WorkCalendarId");

                    b.ToTable("WorkShifts");
                });

            modelBuilder.Entity("Sgs.Attendance.Reports.Models.WorkShift", b =>
                {
                    b.HasOne("Sgs.Attendance.Reports.Models.WorkCalendar", "WorkCalendar")
                        .WithMany("WorkShifts")
                        .HasForeignKey("WorkCalendarId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
