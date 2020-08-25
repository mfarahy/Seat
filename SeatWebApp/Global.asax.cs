using Exir.Framework.Common;
using Exir.Framework.Uie.Bocrud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace SeatWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Initializer.Application_Start();

            System.Web.Helpers.AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

            MvcHandler.DisableMvcResponseHeader = true;
        }
        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (!Request.Path.EndsWith("/CheckSession") && (
                Context.Handler is IRequiresSessionState ||
                Context.Handler is IReadOnlySessionState
                ))
            {
                Session["__lastAccess"] = DateTime.UtcNow;
                var sessionProvider = ObjectRegistry.GetObject<ISessionProvider>();
                sessionProvider.Upsert("__lastAccess", DateTime.UtcNow);
            }
        }
        protected void Application_EndRequest()
        {
            TransactionContext.Current.Clear();
        }
    }
}
