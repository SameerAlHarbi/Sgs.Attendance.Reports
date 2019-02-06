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
                case ContractWorkTime.ShiftB:
                    return "الوردية ب";
                case ContractWorkTime.ShiftC:
                    return "الوردية ج";
                case ContractWorkTime.ShiftD:
                    return "الوردية د";
                case ContractWorkTime.ShiftE:
                    return "الوردية هـ";
                default:
                    return "موظفي الصباح";
            }
        }
    }
}
