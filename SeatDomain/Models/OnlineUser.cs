using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exir.Framework.Common;
using Exir.Framework.Common.Serialization;
using Newtonsoft.Json;

namespace SeatDomain.Models
{
    [Serializer(typeof(NewtonJsonSerializer))]
    public class OnlineUser : VirtualEntityBase
    {
        public OnlineUser() : base(new BaseField() { Name = nameof(SessionId), PropertyType = typeof(string) }) { }

        [JsonProperty]
        public string SessionId { get; set; }
        [JsonProperty]
        public string UserName { get; set; }
        [JsonProperty]
        public DateTime SessionStartedDate { get; set; }
        [JsonProperty]
        public DateTime LastActionDate { get; set; }
        [JsonProperty]
        public string IPAddress { get; set; }
        [JsonProperty]
        public bool IsExpired { get; set; }
    }
}
