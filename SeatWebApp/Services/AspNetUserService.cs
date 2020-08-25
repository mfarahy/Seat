using Exir.Framework.Common;
using Exir.Framework.Service;
using Microsoft.AspNet.Identity;
using SeatDomain.Exceptions;
using SeatDomain.Models;
using SeatDomain.Services;
using SeatWebApp.Security;
using System;
using System.Linq;
using M = SeatDomain.Models;

namespace SeatWebApp.Services
{
   
    public partial class AspNetUserService : CrudService<M.AspNetUser>, IAspNetUserService
    {

        protected new IAspNetUserService This { get { return base.This<IAspNetUserService>(); } }

        public IUserManager UserManager { get; }

        public AspNetUserService(IRepository<M.AspNetUser> repository, IUserManager userManager) : base(repository)
        {
            UserManager = userManager;
        }

        public override object Save(M.AspNetUser entity)
        {
            if (entity.HasKey())
            {
                var result = base.Save(entity);

                addRoles(entity, entity.Id);

                return result;
            }
            else
            {
                var user = new Models.AppUser
                {
                    Email = entity.Email,
                    PasswordHash = UserManager.PasswordHasher.HashPassword(entity.Password),
                    PhoneNumber = entity.PhoneNumber,
                    UserName = entity.UserName
                };

                var result = UserManager.Create(user);
                if (!result.Succeeded)
                    throw new SeatException(String.Join(",", result.Errors));

                addRoles(entity, user.Id);

                entity.Id = user.Id;
                return entity;
            }
        }

        private void addRoles(M.AspNetUser entity, int userId)
        {
            if (entity.AspNetRoles.Count > 0)
            {
                var result = UserManager.AddToRoles(userId, entity.AspNetRoles.Select(x => x.Name).ToArray());
                if (!result.Succeeded)
                    throw new SeatException(String.Join(",", result.Errors));
            }
        }

        public AspNetUser FindByName(string username)
        {
            return GetDefaultQuery()
                .Where(x => x.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
        }

        public AspNetUser GetCurrent()
        {
            string cusername = Authenticater.Value.CurrentIdentity.Name;
            return GetDefaultQuery()
                .Where(x => x.UserName.Equals(cusername, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
        }
    }

}
