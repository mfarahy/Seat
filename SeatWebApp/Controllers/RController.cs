using System.Web.Mvc;
using System.Linq;
using SeatDomain.Services;
using System;
using Exir.Framework.Service;
using Exir.Framework.Common;
using System.Web;
using Exir.Framework.Uie;
using System.Collections.Specialized;
using Exir.Framework.Uie.Contracts;

namespace SeatWebApp.Controllers
{
    public class RController : Controller
    {
        private Lazy<IShortLinkService> _shortLinkService;
        public RController()
        {
            _shortLinkService = new Lazy<IShortLinkService>(() => StaticServiceFactory.Create<IShortLinkService>());
        }


        public ActionResult Index()
        {
            string[] frags = Request.RawUrl.Split('?');
            if (frags.Length == 2)
            {
                var link = _shortLinkService.Value.Refer(frags[1]);

                if (String.IsNullOrEmpty(link))
                    return HttpNotFound();

                return Redirect(link);
            }
            else
                return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Register(string link, string title)
        {
            if (!String.IsNullOrEmpty(link))
            {
                var code = _shortLinkService.Value.Register(link, title, true);
                var shortLink = MyUrlHelper.GetUrl("R", "", null, true) + "?" + code;
                return Content(shortLink);
            }
            else
                return Content(String.Empty);
        }
    }
}