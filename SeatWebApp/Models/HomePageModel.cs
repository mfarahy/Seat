using SeatDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SeatWebApp.Models
{
    public class HomePageModel
    {
        public Instrument[] TopIndecies { get; internal set; }
    }
}