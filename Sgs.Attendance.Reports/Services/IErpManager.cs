﻿using Sgs.Attendance.Reports.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Services
{
    public interface IErpManager
    {
        Task<List<EmployeeInfoViewModel>> GetEmployeesInfo(IEnumerable<int> employeesIds = null);

        Task<List<EmployeeInfoViewModel>> GetEmployeesInfo(string departmentCode);
    }
}
