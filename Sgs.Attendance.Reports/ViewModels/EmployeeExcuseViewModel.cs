using Sameer.Shared;
using Sgs.Attendance.Reports.Models;
using System;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class EmployeeExcuseViewModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public DateTime RegisterDate { get; set; }

        public DateTime ExcueseDate { get; set; }

        public string ExcueseDateHijri => ExcueseDate.ConvertToString(true, true, true) + "هـ ";

        public ExcuseType ExcuseType { get; set; }

        public string ExcuseTypeText
        {
            get
            {

                string typeText = ExcuseType == ExcuseType.CheckIn ? "دخول" : "خروج";

                if (ExcuseHours > 0)
                    typeText = $"{typeText} محدود";

                return typeText;
            }
        }

        public double? ExcuseHours { get; set; }

        public TimeSpan? ExcuseHoursTime
        {
            get
            {
                return ExcuseHours.HasValue ? ExcuseHours.Value.ConvertToTime() : default(TimeSpan?);
            }
            set
            {
                ExcuseHours = value.HasValue ? value.Value.ConvertToDouble() : default(double?);
            }
        }

        public string Note { get; set; }

        public int UserId { get; set; }
    }
}
