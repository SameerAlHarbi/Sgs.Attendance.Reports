namespace Sgs.Attendance.Reports.Models
{
    public enum AttendanceProof
    {
        RequiredInOut,
        RequiredIn,
        Exempted
    }

    public enum ExcuseType
    {
        CheckIn,
        CheckOut
    }

    public enum ContractWorkTime
    {
        MorningWork,
        MorningShift,
        EveningShift,
        NightShift,
        OffShift
    }

}