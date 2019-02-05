using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sgs.Attendance.Reports.ViewModels;

namespace Sgs.Attendance.Reports.Services
{
    public class ErpManager : IErpManager
    {
        private readonly HttpClient _client;

        public ErpManager(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<EmployeeInfoViewModel>> GetEmployeesInfo(IEnumerable<int> employeesIds = null)
        {
            try
            {
                string url = "Employees";

                if (employeesIds != null || employeesIds.Count() > 0)
                {
                    string employeesIdsString = string.Join(',', employeesIds);
                    url += $"?employeesIds={employeesIdsString}";
                }

                HttpResponseMessage response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<EmployeeInfoViewModel>>(content);
                    return results;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception("Data not found !!");
                }
                else //Else in case of BadRequest for not found data or InternalServerError
                {
                    throw new Exception("Internal Server Error");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<EmployeeInfoViewModel>> GetEmployeesInfo(string departmentCode)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync($"HrVacationsEmployees?deptCode={departmentCode}");
                if(response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<EmployeeInfoViewModel>>(content);

                    if (results.Count < 1)
                    {
                        return results;
                    }

                    var departmentsIds = results.Select(e => e.DepartmentId).Distinct();

                    foreach (var deptId in departmentsIds)
                    {
                        var response2 = await _client.GetAsync($"DeptManager/{deptId}");
                        if (response2.IsSuccessStatusCode)
                        {
                            content = await response2.Content.ReadAsStringAsync();
                            var resultManager = JsonConvert.DeserializeObject<EmployeeInfoViewModel>(content);

                            foreach (var item in results.Where(e => e.DepartmentId == deptId))
                            {
                                item.ReportToId = resultManager?.EmployeeId;
                                if (item.ReportToId != null)
                                {
                                    item.ReportTo = results.FirstOrDefault(e => e.EmployeeId == item.ReportToId);
                                    item.ReportToId = item.ReportTo?.EmployeeId;
                                }

                                if (item.ReportToId != null && item.ReportToId.Value == item.EmployeeId)
                                {
                                    var response3 = await _client.GetAsync($"OriginalManager/{item.EmployeeId}");
                                    if (response3.IsSuccessStatusCode)
                                    {
                                        string content2 = await response3.Content.ReadAsStringAsync();
                                        var superManager = JsonConvert.DeserializeObject<EmployeeInfoViewModel>(content2);

                                        item.ReportToId = superManager?.EmployeeId;
                                        if (item.ReportToId != null && item.ReportToId.Value != item.EmployeeId)
                                        {
                                            item.ReportTo = results.FirstOrDefault(e => e.EmployeeId == item.ReportToId);
                                            item.ReportToId = item.ReportTo?.EmployeeId;
                                            continue;
                                        }
                                    }

                                    item.ReportTo = null;
                                    item.ReportToId = null;
                                }
                            }
                        }
                    }

                    return results;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception("Data not found !!");
                }
                else //Else in case of BadRequest for not found data or InternalServerError
                {
                    throw new Exception("Internal Server Error");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
