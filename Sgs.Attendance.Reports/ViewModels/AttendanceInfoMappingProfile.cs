using AutoMapper;
using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class AttendanceInfoMappingProfile : Profile
    {
        public AttendanceInfoMappingProfile()
        {
            CreateMap<EmployeeInfo, EmployeeInfoViewModel>()
                .ReverseMap();
        }
    }
}
