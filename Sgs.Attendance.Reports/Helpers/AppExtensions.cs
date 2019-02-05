using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.Helpers
{
    public static class AppExtensions
    {
        public static string GetText(this AttendanceProof attendanceProof)
        {
            switch (attendanceProof)
            {
                case AttendanceProof.RequiredIn:
                    return "بصمة دخول";
                case AttendanceProof.Exempted:
                    return "معفي";
                default:
                    return "بصمة دخول و خروج";
            }
        }

        public static string GetText(this ContractWorkTime contractWorkTime)
        {
            switch (contractWorkTime)
            {
                case ContractWorkTime.MorningShift:
                    return "وردية الصباح";
                case ContractWorkTime.EveningShift:
                    return "وردية المساء";
                case ContractWorkTime.NightShift:
                    return "وردية الليل";
                default:
                    return "دوام الصباح";
            }
        }
    }
}
