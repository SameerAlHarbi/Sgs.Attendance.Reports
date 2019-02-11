using AutoMapper;
using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class AttendanceInfoMappingProfile : Profile
    {
        public AttendanceInfoMappingProfile()
        {
            CreateMap<EmployeeCalendar, EmployeeInfoViewModel>()
                .ReverseMap();

            CreateMap<EmployeeExcuse, EmployeeExcuseViewModel>().ReverseMap();

            CreateMap<ProcessingRequest, ProcessingRequestViewModel>().ReverseMap();

            CreateMap<EmployeeDayReport, EmployeeDayReportViewModel>().ReverseMap();
        }
    }
}
