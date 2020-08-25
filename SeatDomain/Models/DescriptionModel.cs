using Exir.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Models
{
    public class DescriptionModel : VirtualEntityBase
    {
        public DescriptionModel() { }
        public string SelectedRows { get; set; }
        public string Context { get; set; }
        public string Description { get; set; }
    }
}
