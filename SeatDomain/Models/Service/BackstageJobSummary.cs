using Exir.Framework.Common;
using Exir.Framework.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Models.Service
{
    [Serializer(typeof(JilSerializer))]

    public class BackstageJobSummary : VirtualEntityBase
    {
    }
}
