using System.Collections.Generic;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class DepartmentInfoViewModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string ParentCode { get; set; }

        public string ParentName { get; set; }

        public DepartmentInfoViewModel ParentDepartmentInfo { get; set; }

        public int? ManagerId { get; set; }

        public string ManagerName { get; set; }

        public string ManagerPosition { get; set; }

        public List<DepartmentInfoViewModel> ChildDepartmentsList { get; set; }

        public int ChildsCount { get; set; }
    }
}
