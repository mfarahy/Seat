using Exir.Framework.Common;
using Exir.Framework.Common.Serialization;
using SeatDomain.Services;
using System.Web.Mvc;

namespace SeatWebApp.Controllers
{
    [Authorize]
    public class DashboardNoteController : Controller
    {
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.Any, Duration = 21600, VaryByCustom ="User")]
        public ActionResult GetList()
        {
            var noteSrv = StaticServiceFactory.Create<IDashboardNoteService>();
            var notes = noteSrv.GetVisibleNotesByCurrentUser();
            JilSerializer ser = new JilSerializer();
            return Content(ser.SerializeToString(notes), "application/json");
        }

        public ActionResult Remove(int id)
        {
            var noteSrv = StaticServiceFactory.Create<IDashboardNoteService>();
            noteSrv.Close(id);
            return Content("OK");
        }
    }
}