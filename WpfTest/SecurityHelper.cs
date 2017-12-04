using System.Security;
using System.Security.Permissions;

namespace WpfTest
{
    internal static class SecurityHelper
    {
        [SecuritySafeCritical]
        internal static bool CheckUnmanagedCodePermission()
        {
            try
            {
                SecurityHelper.DemandUnmanagedCode();
            }
            catch (SecurityException)
            {
                return false;
            }

            return true;
        }

        [SecurityCritical]
        internal static void DemandUnmanagedCode()
        {
            if (_unmanagedCodePermission == null)
            {
                _unmanagedCodePermission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
            }
            _unmanagedCodePermission.Demand();
        }
        static SecurityPermission _unmanagedCodePermission = null;

    }
}
