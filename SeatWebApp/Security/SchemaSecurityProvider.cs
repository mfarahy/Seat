using Exir.Framework.Common;
using Exir.Framework.Security;
using Exir.Framework.Security.SchemaSecurity;
using Exir.Framework.Security.SchemaSecurity.Support;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SeatWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SeatWebApp.Security
{
    public class SchemaSecurityProvider : ISchemaSecurityProvider
    {
        private Lazy<IAuthenticaterProvider> authenticater;


        public SchemaSecurityProvider(IAuthenticaterProvider authenticater)
        {
            this.authenticater = new Lazy<IAuthenticaterProvider>(() => authenticater);
        
        }

        public SchemaSecurityProvider()
        {
            this.authenticater = new Lazy<IAuthenticaterProvider>(() => ObjectRegistry.GetObject<IAuthenticaterProvider>());
        }

        public bool CanDo(string username, string operation, bool throwExceptionIfNotExist)
        {
            if (String.IsNullOrEmpty(username))
            {
                return SecurityConfig.Instance.Roles.Cast<RoleElement>()
               .Where(x => String.Compare(x.Key, Constants.KnownRoles.Anonymouse, true) == 0 && x.Operations != null &&
               x.Operations.Cast<OperationElement>().Any(y => String.Compare(y.Key, operation, true) == 0)).Any();
            }
            var roles = FindRoles(operation);
            if (roles.Length == 0) return true;
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            AppUser user = userManager.FindByName(username);
            if (user == null) return false;
            var userRoles = userManager.GetRoles(user.Id);
            return roles.Intersect(userRoles).Any();
        }

        private string[] FindRoles(string operation)
        {
            return SecurityConfig.Instance.Roles.Cast<RoleElement>()
                          .Where(x => x.Operations != null && x.Operations.Cast<OperationElement>().Any(y => String.Compare(y.Key, operation, true) == 0))
                          .Select(x => x.Key)
                          .Distinct()
                          .ToArray();
        }

        public bool CanDo(string operation, bool throwExceptionIfNotExist)
        {
            var cusername = authenticater.Value.CurrentIdentity.Name;
            return CanDo(cusername, operation, throwExceptionIfNotExist);
        }

        public void ClearCache()
        {
        }

        public void ClearCache(object sender, EventArgs e)
        {
        }

        public IEnumerable<Exir.Framework.Security.SchemaSecurity.IRole> GetMyRoles(string username)
        {
            return GetUserRoles(username).Select(x => new Role(x));
        }


        public IEnumerable<IOperation> GetOperations()
        {
            return SecurityConfig.Instance.Roles.Cast<RoleElement>()
                .Where(x => x.Operations != null)
                .SelectMany(x => x.Operations.Cast<OperationElement>().Select(y => y.Key))
                .Distinct()
                .Select(x => new Operation()
                {
                    Key = x,
                    ID = x,
                    ItemType = AuthorizationItemTypes.Operation
                })
                .ToList();
        }

        public IEnumerable<Exir.Framework.Security.SchemaSecurity.IRole> GetRoles()
        {
            return SecurityConfig.Instance.Roles.Cast<RoleElement>()
                .Select(x => x.Key)
                .Distinct()
                .Select(x => new Role()
                {
                    Key = x,
                    ID = x,
                    ItemType = AuthorizationItemTypes.Role
                })
                .ToList();
        }


        public IEnumerable<IAuthorizationItem> GetUserOperations(string username)
        {
            var roles = GetUserRoles(username).Select(x => x.ID);
            if (roles != null && roles.Any())
            {
                return SecurityConfig.Instance.Roles.Cast<RoleElement>()
                 .Where(x => roles.Contains(x.Key) && x.Operations != null)
                 .SelectMany(x => x.Operations.Cast<OperationElement>().Select(y => y.Key))
                 .Distinct()
                 .Select(x => new Operation()
                 {
                     Key = x,
                     ID = x
                 })
                 .ToList();
            }
            return new IAuthorizationItem[0];
        }

        public IEnumerable<IAuthorizationItem> GetUserRoles(string username)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            AppUser user = userManager.FindByName(username);
            if (user == null) return new IAuthorizationItem[0];
            return userManager.GetRoles(user.Id)
                .Select(x => new AuthorizationItem() { Key = x, ID = x });
        }

        public bool Is(string username, string roleKey)
        {
            if (String.IsNullOrEmpty(username))
                return String.Compare(roleKey, Constants.KnownRoles.Anonymouse, true) == 0;
            else
            {
                var userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
                AppUser user = userManager.FindByName(username);
                if (user == null) return false;
                return userManager.IsInRole(user.Id, roleKey);
            }
        }

        public bool Is(string roleKey)
        {
            if (!authenticater.Value.CurrentIdentity.IsAuthenticated)
                return String.Compare(roleKey, Constants.KnownRoles.Anonymouse, true) == 0;
            else
            {
                var userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
                var cusername = authenticater.Value.CurrentIdentity.Name;
                AppUser user = userManager.FindByName(cusername);
                if (user == null) return false;
                return userManager.IsInRole(user.Id, roleKey);
            }
        }

        public void RemoveAssignment(string username, string authorizationItemKey)
        {
            SetAccess(username, authorizationItemKey, AccessResults.Deny, null);
        }

        public void RemoveItem(string itemKey)
        {
            throw new NotSupportedException();
        }

        public void SaveItem(IAuthorizationItem item)
        {
            throw new NotSupportedException();
        }

        public void SetAccess(string subject, string authorizationItemKey, AccessResults result, string application)
        {
            _setAccess(subject, authorizationItemKey, result);
        }

        private void _setAccess(string subject, string authorizationItemKey, AccessResults result)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            AppUser user = userManager.FindByName(subject);
            if (user != null)
            {
                if (GetRoles().Any(x => x.Key == authorizationItemKey))
                {
                    userManager.AddToRole(user.Id, authorizationItemKey);
                }
                else
                {
                    user.Access = result == AccessResults.Allow ? user.Access.TryAppend(authorizationItemKey, ',') : user.Access.TryRemove(authorizationItemKey, ',');
                    userManager.Update(user);
                }
            }
        }

        public void SetAccess(string owener, string subject, string authorizationItemKey, AccessResults result, string application)
        {
            _setAccess(subject, authorizationItemKey, result);
        }

        public void SetAccess(string owener, string subject, string authorizationItemKey, AccessResults result)
        {
            _setAccess(subject, authorizationItemKey, result);
        }
    }
}