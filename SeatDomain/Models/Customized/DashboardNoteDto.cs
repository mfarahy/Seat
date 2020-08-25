using Exir.Framework.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Models
{
    [Serializer(typeof(JilSerializer))]

    public class DashboardNoteDto
    {
        public string Text { get; set; }
        public string Color { get; set; }
        public int Id { get; set; }
    }
}
