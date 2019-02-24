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
        Default,
        ShiftA,
        ShiftB,
        ShiftC,
        ShiftD,
        ShiftH  
    }

}