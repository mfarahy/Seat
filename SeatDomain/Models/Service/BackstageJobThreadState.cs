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

    public class BackstageJobThreadState
    {
        public int AvailableThreads { get; set; }
        public int MaxThreads { get; set; }
        public int MinThreads { get; set; }
        public string QueueState { get; set; }
        public JobGroupState[] JobGroups { get; set; }

    }

    public class JobGroupState : VirtualEntityBase
    {
        public string Action { get; set; }
        public int Count { get; set; }
    }
}
