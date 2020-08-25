using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SeatWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SeatWebApp.Security
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(() => new MyDbContext("LocalSqlServer"));
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<RoleManager<AppRole, int>>((options, context) =>
            {
                return new RoleManager<AppRole, int>(new AppRoleStore(new MyDbContext("LocalSqlServer")));
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Home/Login"),
            });
        }
    }

}