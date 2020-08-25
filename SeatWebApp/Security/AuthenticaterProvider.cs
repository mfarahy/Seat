using Exir.Framework.Common;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SeatWebApp.Models;
using SeatWebApp.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SeatWebApp.Security
{
    public interface IAspNetAuthenticaterProvider : IAuthenticaterProvider
    {
        bool CreateUser(string userName, string password, string email, string mobileNo, out IEnumerable<string> errors);
    }

    public class AuthenticaterProvider : IAuthenticaterProvider
    {
        public int MinPasswordLength => 6;

        public IIdentity CurrentIdentity
        {
            get
            {
                if (HttpContext.Current == null)
                    return Thread.CurrentPrincipal?.Identity;
                else
                    return HttpContext.Current.User?.Identity;
            }
        }

        public void Authenticate(string userName, string password)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            var authManager = HttpContext.Current.GetOwinContext().Authentication;

            AppUser user = userManager.Find(userName, password);
            if (user != null)
            {
                var ident = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
            }
        }

        public void Authenticate(string userName)
        {
            if (HttpContext.Current == null)
            {
                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(userName), null);
            }
            else
            {
                var userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
                var authManager = HttpContext.Current.GetOwinContext().Authentication;

                AppUser user = userManager.FindByName(userName);
                if (user != null)
                {
                    var ident = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                    authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
                }
            }
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            AppUser user = userManager.FindByName(userName);
            var result = userManager.ChangePassword(user.Id, oldPassword, newPassword);
            return result.Succeeded;
        }

        public bool ChangePassword(string userName, string newPassword)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            AppUser user = userManager.FindByName(userName);
            var token = userManager.GeneratePasswordResetToken(user.Id);
            var result = userManager.ResetPassword(user.Id, token, newPassword);
            return result.Succeeded;
        }

        public void ClearCache(object sender, EventArgs e)
        {
        }
        public bool CreateUser(string userName, string password, string email, out IEnumerable<string> errors)
        {
            return CreateUser(userName, password, email, null, out errors);
        }
        public bool CreateUser(string userName, string password, string email, string mobileNo, out IEnumerable<string> errors)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            var result = userManager.Create(new AppUser
            {
                UserName = userName,
                PasswordHash = userManager.PasswordHasher.HashPassword(password),
                Email = email,
                PhoneNumber = mobileNo
            });
            errors = result.Errors;
            return result.Succeeded;
        }

        public Dictionary<string, Dictionary<string, object>> GetUsers()
        {
            throw new NotSupportedException();
        }

        public void RemoveUser(string userName)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            AppUser user = userManager.FindByName(userName);
            userManager.Delete(user);
        }

        public bool ValidateUser(string userName, string password)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            return userManager.Find(userName, password) != null;
        }

        public void SignOut()
        {
            var authManager = HttpContext.Current.GetOwinContext().Authentication;

            authManager.SignOut();
        }
    }
}