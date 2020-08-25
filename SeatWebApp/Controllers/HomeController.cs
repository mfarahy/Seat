using Exir.Framework.Common;
using SeatDomain.Models;
using SeatDomain.Services;
using SeatWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SeatWebApp.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var isrv = StaticServiceFactory.Create<IInstrumentService>();
            return View(new HomePageModel
            {
                TopIndecies = isrv.GetTopIndecies()
            });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}