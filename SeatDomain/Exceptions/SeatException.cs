using Exir.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Exceptions
{


    public class SeatException : ExirException
    {
        public string Code { get; set; }
        public SeatException(string message, params object[] args) : base(String.Format(message, args)) { }
        public SeatException(bool useResource, string section, string messageCode, params object[] args) :
            base(String.Format(R(section, messageCode) ?? messageCode, args))
        {
            Code = messageCode?.ToUpper();
        }

        private static string R(string section, string code)
        {
            var resourceProvider = ObjectRegistry.GetObject<IResourceProvider>();
            return resourceProvider.GetResource(section, code);
        }

    }
}
