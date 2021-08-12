using Microsoft.AspNetCore.Authentication;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Helpers
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            try
            {
                var wi = (WindowsIdentity)principal.Identity;

                if (wi.IsAuthenticated)
                {
                    if (wi.Name.Contains("1143") || wi.Name.Contains("917") || wi.Name.Contains("865") || wi.Name.Contains("1550") || wi.Name.Contains("1113") || wi.Name.Contains("25"))
                    {
                        wi.AddClaim(new Claim(wi.RoleClaimType, "Admin"));
                    }
                    else if (wi.Name.Contains("1550") || wi.Name.Contains("1080") || wi.Name.Contains("492") ||
                        wi.Name.Contains("1133") || wi.Name.Contains("574") || wi.Name.Contains("1235"))
                    {
                        wi.AddClaim(new Claim(wi.RoleClaimType, "AttendanceDepartment"));
                    }
                }
                return Task.FromResult(principal);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
