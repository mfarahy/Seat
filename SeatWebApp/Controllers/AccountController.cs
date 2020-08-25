using Exir.Framework.Common;
using Exir.Framework.Common.Caching;
using Exir.Framework.Security;
using Exir.Framework.Security.Cryptography;
using Exir.Framework.Security.SchemaSecurity;
using Exir.Framework.Uie.Adapter;
using Exir.Framework.Uie.Bocrud;
using Microsoft.AspNet.Identity.Owin;
using SeatDomain.Services;
using SeatWebApp.Models;
using SeatWebApp.Security;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SeatWebApp.Controllers
{
    public class AccountController : Controller
    {

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var authProvider = ObjectRegistry.GetObject<IAuthenticaterProvider>();
                if (authProvider.ValidateUser(login.UserName, login.Password))
                {
                    authProvider.Authenticate(login.UserName, login.Password);
                    return Redirect(login.ReturnUrl ?? Url.Action("Index", "Home"));
                }
            }
            ModelState.AddModelError("", "Invalid username or password");
            return View(login);
        }

        [HttpGet]
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(SignupModel model)
        {
            ModelState.SetValue<Captcha>(nameof(model.CaptchaCode), model);

            if (ModelState.IsValid)
            {
                var authenticater = ObjectRegistry.GetObject<IAspNetAuthenticaterProvider>();
                IEnumerable<string> errors;

                if (!authenticater.CreateUser(model.UserName, model.Password, model.Email, model.MobileNo, out errors))
                {
                    ModelState.AddModelError("RegistrationError", string.Join(",", errors));
                }
                else
                {
                    var schemaProvider = ObjectRegistry.GetObject<ISchemaSecurityProvider>();
                    schemaProvider.SetAccess(model.UserName, Constants.KnownRoles.normal, AccessResults.Allow, null);
                }
            }
            return View(nameof(Login));
        }

        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string email, string mobileNo)
        {
            return Json(new { ResponseMsg = "" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult SignOut()
        {
            sendSignoutSignal();

            var authProvider = ObjectRegistry.GetObject<IAuthenticaterProvider>();
            authProvider.SignOut();

            return Redirect("/");
        }


        private void sendSignoutSignal()
        {
            var cache = ObjectRegistry.GetObject<ICacheProvider>(CacheConstants.DefaultCacheObject, true);
            var authenticaterProvider = ObjectRegistry.GetObject<IAuthenticaterProvider>();
            var cusername = authenticaterProvider.CurrentIdentity.Name?.ToLower();
            if (!String.IsNullOrEmpty(cusername))
                cache.SendMessage(CacheConstants.KnownChannels.Cache(), CacheConstants.Cache_Messages.Signout() + cusername, cusername);
        }

        public ActionResult ConfirmEmail(string code)
        {
            var text = EmbedRsaCryptoService.XorDecryptSafeBase64(code);
            var frags = text.Split('|');
            var userId = int.Parse(frags[0]);

            var userSrv = StaticServiceFactory.Create<IAspNetUserService>();
            var user = userSrv.GetEntity(userId, null);
            bool result = false;
            if (String.Compare(user.Email, frags[1], true) == 0)
            {
                user.EmailConfirmed = true;
                userSrv.Save(user);
                result = true;
            }
            var msgSrv = StaticServiceFactory.Create<ISystemMessageService>();
            string message = null;
            if (result)
                message = msgSrv.GetByCode(SeatDomain.Constants.Messages.email_confirmed);
            else
                message = msgSrv.GetByCode(SeatDomain.Constants.Messages.email_not_confirmed);

            return View(message);
        }

    }
}