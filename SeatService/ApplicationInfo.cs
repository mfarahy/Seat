using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SeatService
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
    }
}
