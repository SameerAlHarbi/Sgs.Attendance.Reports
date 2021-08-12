using Newtonsoft.Json;
using Sgs.Attendance.Reports.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Sameer.Shared.Data;
using Sameer.Shared.Mvc;
using Sameer.Shared;

namespace Sgs.Attendance.Reports.Services
{
    public class ErpManager : IErpManager
    {
        private readonly HttpClient _client;

        public ErpManager(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<EmployeeInfoViewModel>> GetEmployeesInfo(IEnumerable<int> employeesIds = null,string employeeName = null
            , bool? active = true, string fromDate = null, string toDate = null, string dateFormat = "yyyy-MM-dd")
        {
            try
            {
                string url = "Employees";

                DateTime? fromDateObject = fromDate?.TryParseToDate(dateFormat);
                DateTime? toDateObject = toDate?.TryParseToDate(dateFormat);

                if (employeesIds != null && employeesIds.Count() > 0)
                {
                    string employeesIdsString = string.Join(',', employeesIds);
                    url += $"?employeesIds={employeesIdsString}";
                }

                if(employeeName != null)
                {
                    url += url.IndexOf('?') >= 0 ? "&" : "?";
                    url += $"employeeName={employeeName}";
                }

                if (active.HasValue)
                {
                    url += url.IndexOf('?') >= 0 ? "&" : "?";
                    url += $"active={active.Value}";
                }

                if (fromDateObject.HasValue)
                {
                    url += url.IndexOf('?') >= 0 ? "&" : "?";
                    url += $"fromDate={fromDateObject.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US"))}";
                }

                if (toDateObject.HasValue)
                {
                    url += url.IndexOf('?') >= 0 ? "&" : "?";
                    url += $"toDate={toDateObject.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US"))}";
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

        public async Task<List<ShortEmployeeInfoViewModel>> GetShortEmployeesInfo(IEnumerable<int> employeesIds = null, string employeeName = null, bool? active = true, string fromDate = null, string toDate = null, string dateFormat = "yyyy-MM-dd")
        {
            try
            {
                string url = "Employees/shortinfo";

                DateTime? fromDateObject = fromDate?.TryParseToDate(dateFormat) ?? new DateTime(2018,1,1);
                DateTime? toDateObject = toDate?.TryParseToDate(dateFormat);

                if (employeesIds != null && employeesIds.Count() > 0)
                {
                    string employeesIdsString = string.Join(',', employeesIds);
                    url += $"?employeesIds={employeesIdsString}";
                }

                if (employeeName != null)
                {
                    url += url.IndexOf('?') >= 0 ? "&" : "?";
                    url += $"employeeName={employeeName}";
                }

                if (fromDateObject.HasValue)
                {
                    url += url.IndexOf('?') >= 0 ? "&" : "?";
                    url += $"fromDate={fromDateObject.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US"))}";
                }

                if (toDateObject.HasValue)
                {
                    url += url.IndexOf('?') >= 0 ? "&" : "?";
                    url += $"toDate={toDateObject.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US"))}";
                }

                HttpResponseMessage response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<ShortEmployeeInfoViewModel>>(content);
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

        public async Task<List<EmployeeInfoViewModel>> GetDepartmentEmployeesInfo(string departmentCode, bool? active = true, string fromDate = null, string toDate = null, string dateFormat = "yyyy-MM-dd")
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

        private void setDepartmentId(DepartmentInfoViewModel department)
        {
            department.Id = int.Parse(department.Code);
            foreach (var childDepartment in department.ChildDepartmentsList)
            {
                childDepartment.ParentDepartmentInfo = department;
                setDepartmentId(childDepartment);
            }
        }

        public async Task<IEnumerable<DepartmentInfoViewModel>> GetAllDepartmentsInfo()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("departments");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<DepartmentInfoViewModel>>(content);

                    foreach (var department in results)
                    {
                        setDepartmentId(department);
                    }

                    var sgs = results?.FirstOrDefault()?.ChildDepartmentsList.FirstOrDefault(d => d.Id == 130);
                    if (sgs != null)
                    {
                        sgs.ParentDepartmentInfo = null;
                        sgs.ParentCode = null;
                        sgs.ParentName = null;

                        results = new List<DepartmentInfoViewModel> { sgs };
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

        public async Task<IEnumerable<DepartmentInfoViewModel>> GetFlatDepartmentsInfo()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("FlatDepartments");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<DepartmentInfoViewModel>>(content);

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

        public async Task<IEnumerable<DepartmentInfoViewModel>> GetChildsDepartmentsInfo(string parentDepartmentCode)
        {
            try
            {
                string url = !string.IsNullOrWhiteSpace(parentDepartmentCode) ? $"ChildDepartments?deptCode={parentDepartmentCode}" : "ChildDepartments?deptCode=00000130";
                HttpResponseMessage response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<DepartmentInfoViewModel>>(content);

                    foreach (var item in results)
                    {
                        item.Id = int.Parse(item.Code);
                        item.ParentDepartmentInfo = results.FirstOrDefault(d => d.Code == item.ParentCode);
                        item.ChildDepartmentsList.AddRange(results.Where(d => d.ParentCode == item.Code));
                    }

                    //if (string.IsNullOrWhiteSpace(parentDepartmentCode))
                    //{
                    //    var sgs = results?.FirstOrDefault(d => d.Id == 130);
                    //    if (sgs != null)
                    //    {
                    //        sgs.ParentDepartmentInfo = null;
                    //        sgs.ParentCode = null;
                    //        sgs.ParentName = null;

                    //        results = new List<DepartmentInfoViewModel> { sgs };
                    //    }
                    //}


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

        public async Task<List<VacationViewModel>> GetAllVacations(DateTime fromDate, DateTime toDate, IEnumerable<int> employeesIds = null)
        {
            try
            {
                string url = $"vacations?fromDate={fromDate.Date.ToString("yyyy-MM-dd", new CultureInfo("en-US"))}&toDate={toDate.Date.ToString("yyyy-MM-dd", new CultureInfo("en-US"))}";

                if (employeesIds != null && employeesIds.Count() > 0)
                {
                    string employeesIdsString = string.Join(',', employeesIds);
                    url += $"&employeesIds={employeesIdsString}";
                }

                HttpResponseMessage response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<VacationViewModel>>(content);
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

        public async Task<List<AttendanceTransactionViewModel>> GetAllTransaction(DateTime fromDate, DateTime toDate, IEnumerable<int> employeesIds = null)
        {
            try
            {
                string url = $"RawAttendanceTransactions?fromDate={fromDate.Date.ToString("yyyy-MM-dd", new CultureInfo("en-US"))}&toDate={toDate.Date.ToString("yyyy-MM-dd", new CultureInfo("en-US"))}";

                if (employeesIds != null && employeesIds.Count() > 0)
                {
                    string employeesIdsString = string.Join(',', employeesIds);
                    url += $"&employeesids={employeesIdsString}";
                }

                HttpResponseMessage response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<AttendanceTransactionViewModel>>(content);
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

        public async Task<List<VacationRequestViewModel>> GetAllOpenDelegations(DateTime fromDate, DateTime toDate, IEnumerable<int> employeesIds = null)
        {
            try
            {
                string url = $"delegations?fromDate={fromDate.Date.ToString("yyyy-MM-dd", new CultureInfo("en-US"))}&toDate={toDate.Date.ToString("yyyy-MM-dd", new CultureInfo("en-US"))}&requestStatus=0,1,2";

                if (employeesIds != null && employeesIds.Count() > 0)
                {
                    string employeesIdsString = string.Join(',', employeesIds);
                    url += $"&employeesIds={employeesIdsString}";
                }

                HttpResponseMessage response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<VacationRequestViewModel>>(content);
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

        public async Task<List<VacationRequestViewModel>> GetAllOpenVacations(DateTime fromDate, DateTime toDate, IEnumerable<int> employeesIds = null)
        {
            try
            {
                string url = $"OpenVacationsRequests?fromDate={fromDate.Date.ToString("yyyy-MM-dd", new CultureInfo("en-US"))}&toDate={toDate.Date.ToString("yyyy-MM-dd", new CultureInfo("en-US"))}&requestStatus=0,1,2";

                if (employeesIds != null && employeesIds.Count() > 0)
                {
                    string employeesIdsString = string.Join(',', employeesIds);
                    url += $"&employeesIds={employeesIdsString}";
                }

                HttpResponseMessage response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<VacationRequestViewModel>>(content);
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

        public async Task<bool> NotifiAbsent(int employeeId, DateTime absentDate)
        {
            try
            {
                string absentDateText = absentDate.ToString("yyyy-MM-dd");

                string url = $"AbsentMail?employeeId={employeeId}&absentDate={absentDateText}";

                HttpResponseMessage response = await _client.PostAsync(url,null);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
