using System.Diagnostics;
using System.Security.Principal;

namespace AsyncPipeTransport.Utils
{
    internal static class SecurityUtils
    {
        public static bool IsRunningAsAdministrator()
        {
            // Check if the current user is part of the Administrators group
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

       
    }
}
