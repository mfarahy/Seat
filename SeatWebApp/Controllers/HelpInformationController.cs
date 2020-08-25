using Exir.Framework.Common;
using Exir.Framework.Common.Serialization;
using Exir.Framework.Uie;
using SeatDomain.Services;
using System.Web.Mvc;

namespace SeatWebApp.Controllers
{
    [AllowAnonymous]
    [UrlUserNameIgnore]
    public class HelpInformationController : Controller
    {
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.ServerAndClient, Duration = 3600, VaryByParam = "formName")]
        public string GetFormHelpInfo(string formName)
        {
            var helpInfoService = StaticServiceFactory.Create<IHelpInformationService>();
            var formHelpInfo = helpInfoService.GetHelps(formName);

            NewtonJsonSerializer ser = new NewtonJsonSerializer();
            return ser.SerializeToString(formHelpInfo);
        }
    }
}