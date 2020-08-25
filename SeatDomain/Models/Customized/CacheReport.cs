using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exir.Framework.Common;

namespace SeatDomain.Models.Customized
{
    public class CacheReport : VirtualEntityBase
    {
        public CacheReport() : base(new BaseField()
        {
            Name = nameof(Key),
            PropertyType = typeof(string)
        })
        {
            
        }
        public string Key { get; set; }
        public string HitCount { get; set; }
        public string ExpireDt { get; set; }
        public string Value { get; set; }
    }
}
