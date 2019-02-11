using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Services
{
    public interface IScopedProcessingService
    {
        Task<bool> DoWork();
    }
}
