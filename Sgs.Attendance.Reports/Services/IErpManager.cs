using Sgs.Attendance.Reports.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Services
{
    public interface IErpManager
    {
        Task<List<EmployeeInfoViewModel>> GetEmployeesInfo(IEnumerable<int> employeesIds = null,string employeeName = null);

        Task<List<ShortEmployeeInfoViewModel>> GetShortEmployeesInfo(IEnumerable<int> employeesIds = null, string employeeName = null);

        Task<List<EmployeeInfoViewModel>> GetDepartmentEmployeesInfo(string departmentCode);

        Task<IEnumerable<DepartmentInfoViewModel>> GetAllDepartmentsInfo();

        Task<IEnumerable<DepartmentInfoViewModel>> GetChildsDepartmentsInfo(string parentDepartmentCode);

        Task<List<VacationViewModel>> GetAllVacations(DateTime fromDate, DateTime toDate, IEnumerable<int> employeesIds = null);

        Task<List<AttendanceTransactionViewModel>> GetAllTransaction(DateTime fromDate, DateTime toDate, IEnumerable<int> employeesIds = null);

        Task<List<VacationRequestViewModel>> GetAllOpenDelegations(DateTime fromDate, DateTime toDate, IEnumerable<int> employeesIds = null);

        Task<List<VacationRequestViewModel>> GetAllOpenVacations(DateTime fromDate, DateTime toDate, IEnumerable<int> employeesIds = null);

        Task<bool> NotifiAbsent(int employeeId,DateTime absentDate);
    }
}
