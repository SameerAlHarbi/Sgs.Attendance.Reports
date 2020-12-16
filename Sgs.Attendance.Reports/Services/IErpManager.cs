using Sgs.Attendance.Reports.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Services
{
    public interface IErpManager
    {
        Task<List<EmployeeInfoViewModel>> GetEmployeesInfo(IEnumerable<int> employeesIds = null,string employeeName = null, bool? active = true, string fromDate = null, string toDate = null, string dateFormat = "yyyy-MM-dd");

        Task<List<ShortEmployeeInfoViewModel>> GetShortEmployeesInfo(IEnumerable<int> employeesIds = null, string employeeName = null, bool? active = true);

        Task<List<EmployeeInfoViewModel>> GetDepartmentEmployeesInfo(string departmentCode, bool? active = true);

        Task<IEnumerable<DepartmentInfoViewModel>> GetAllDepartmentsInfo();

        Task<IEnumerable<DepartmentInfoViewModel>> GetFlatDepartmentsInfo();

        Task<IEnumerable<DepartmentInfoViewModel>> GetChildsDepartmentsInfo(string parentDepartmentCode);

        Task<List<VacationViewModel>> GetAllVacations(DateTime fromDate, DateTime toDate, IEnumerable<int> employeesIds = null);

        Task<List<AttendanceTransactionViewModel>> GetAllTransaction(DateTime fromDate, DateTime toDate, IEnumerable<int> employeesIds = null);

        Task<List<VacationRequestViewModel>> GetAllOpenDelegations(DateTime fromDate, DateTime toDate, IEnumerable<int> employeesIds = null);

        Task<List<VacationRequestViewModel>> GetAllOpenVacations(DateTime fromDate, DateTime toDate, IEnumerable<int> employeesIds = null);

        Task<bool> NotifiAbsent(int employeeId,DateTime absentDate);
    }
}
