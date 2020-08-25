using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SeatWebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("assets/*");
            routes.IgnoreRoute("web-assets/*");

            routes.IgnoreRoute("P/{*dynamic}", new { dynamic = @"DYN-.*" });

            routes.MapRoute(
              name: "Bocrud",
              url: "P/{xml}",
              defaults: new { controller = "Bocrud", action = "Index", xml = UrlParameter.Optional }
          );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
