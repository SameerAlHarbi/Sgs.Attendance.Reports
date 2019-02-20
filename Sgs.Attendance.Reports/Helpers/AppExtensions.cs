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
                case ContractWorkTime.ShiftA:
                    return "المجموعة أ";
                case ContractWorkTime.ShiftB:
                    return "المجموعة ب";
                case ContractWorkTime.ShiftC:
                    return "المجموعة ج";
                case ContractWorkTime.ShiftD:
                    return "المجموعة د";
                case ContractWorkTime.ShiftH:
                    return "المجموعة هـ";
                default:
                    return "موظفي الصباح";
            }
        }

        public static int ConvertToInteger(this string text)
        {
            if (text.Length > 0)
            {
                while (true)
                {
                    int result = 0;
                    if (int.TryParse(text, out result))
                    {
                        return result;
                    }
                    if (text.Length > 0)
                    {
                        text = text.Substring(1, text.Length - 1);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return 0;
        }

       
    }
}
