using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain
{
    public static class ApplicationInfo
    {
        private static string _version;
        public static string Version
        {
            get
            {
                if (String.IsNullOrEmpty(_version))
                {
                    var versionInfo = (AssemblyFileVersionAttribute)typeof(ApplicationInfo).Assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute)).First();
                    _version = versionInfo.Version;
                }
                return _version;
            }
        }

        public static bool IsVersionGreaterThanOrEqual(string version)
        {
            var frags = version.Split('.');
            var cversion = Version.Split('.');
            for (int i = 0; i < frags.Length && i < cversion.Length; ++i)
            {
                if (frags[i]== "*") continue;

                var a = int.Parse(cversion[i]);
                var b = int.Parse(frags[i]);
                if (a == b) continue;
                if (a > b) return true;
            }
            return false;
        }
    }
}
