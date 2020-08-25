using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Exir.Framework.Common;
using Exir.Framework.Uie;
using SeatDomain.Models;
using SeatDomain.Services;
using SeatWebApp.Models;
using Exir.Framework.Uie.Adapter;

namespace SeatWebApp.Controllers
{
    public class NewsController : Controller
    {
        // GET: News


        public ActionResult Index(string id)
        {
            var newsSrv = StaticServiceFactory.Create<INewsService>();
            var clearKey = KeyFactory.GetPrimaryKeyValue<int>(id);
            var item = newsSrv.GetWithDetails((int)clearKey.PrimaryKeyValue);
            if (item == null)
                return HttpNotFound();
            return View(item);
        }

        public ActionResult Slide(string id)
        {
            var newsSrv = StaticServiceFactory.Create<INewsService>();
            var clearKey = KeyFactory.GetPrimaryKeyValue<int>(id);
            var item = newsSrv.GetWithDetails((int)clearKey.PrimaryKeyValue);
            if (item == null)
                return HttpNotFound();
            return View(item);
        }


        public ActionResult TilePartial(string categoryCode)
        {
            var newsSrv = StaticServiceFactory.Create<INewsService>();
            var data = newsSrv.GetTopNews(categoryCode);
            return PartialView(data);
        }

        public ActionResult SliderPartial(string categoryCode)
        {
            var newsSrv = StaticServiceFactory.Create<INewsService>();
            var data = newsSrv.GetSlides(categoryCode).ToList();
            return PartialView(data);
        }

        [OutputCache(Duration = 1200, Location = System.Web.UI.OutputCacheLocation.Client)]
        public ActionResult ShowImage(string id)
        {
            var newsSrv = StaticServiceFactory.Create<INewsService>();
            var clearKey = KeyFactory.GetPrimaryKeyValue<int>(id);
            var item = newsSrv.GetImage((int)clearKey.PrimaryKeyValue);
            if (item == null) return null;

            var files = FilePropertyValue.Deserialize(item);
            var image = files.Files[0];
            return File(image.Content, "image/jpeg");
        }
    }
}