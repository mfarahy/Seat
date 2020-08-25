using Exir.Framework.Common;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SeatWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace SeatWebApp.Security
{
    public class AppRoleStore : RoleStore<AppRole, int, AppUserRole>
    {
        public AppRoleStore(MyDbContext context)
            : base(context)
        {
        }
    }
    public class UserStore : UserStore<AppUser, AppRole, int, AppUserLogin, AppUserRole, AppUserClaim>
    {
        public UserStore(string connection) : base(new MyDbContext(connection))
        {
        }
        public UserStore(MyDbContext context) : base(context)
        {
        }
        public UserStore() : base(new MyDbContext())
        {
        }
    }
    public interface IUserManager : IUserManager<AppUser, int>
    { }
    public class AppUserManager : UserManager<AppUser, int>, IUserManager
    {
        public AppUserManager(IUserStore<AppUser, int> store)
            : base(store)
        {
        }
        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            var manager = new AppUserManager(new UserStore(context.Get<MyDbContext>()));

            return manager;
        }

        public IdentityResult AccessFailed(int userId)
        {
            return AsyncHelper.RunSync(() => AccessFailedAsync(userId));
        }

        public IdentityResult AddClaim(int userId, Claim claim)
        {
            return AsyncHelper.RunSync(() => AddClaimAsync(userId, claim));
        }

        public IdentityResult AddLogin(int userId, UserLoginInfo login)
        {
            return AsyncHelper.RunSync(() => AddLoginAsync(userId, login));
        }

        public IdentityResult AddPassword(int userId, string password)
        {
            return AsyncHelper.RunSync(() => AddPasswordAsync(userId, password));
        }

        public IdentityResult AddToRole(int userId, string role)
        {
            return AsyncHelper.RunSync(() => AddToRoleAsync(userId, role));
        }

        public IdentityResult AddToRoles(int userId, params string[] roles)
        {
            return AsyncHelper.RunSync(() => AddToRolesAsync(userId, roles));
        }

        public IdentityResult ChangePassword(int userId, string currentPassword, string newPassword)
        {
            return AsyncHelper.RunSync(() => ChangePasswordAsync(userId, currentPassword, newPassword));
        }

        public IdentityResult ChangePhoneNumber(int userId, string phoneNumber, string token)
        {
            return AsyncHelper.RunSync(() => ChangePhoneNumberAsync(userId, phoneNumber, token));
        }

        public bool CheckPassword(AppUser user, string password)
        {
            return AsyncHelper.RunSync(() => CheckPasswordAsync(user, password));
        }

        public IdentityResult ConfirmEmail(int userId, string token)
        {
            return AsyncHelper.RunSync(() => ConfirmEmailAsync(userId, token));
        }

        public IdentityResult Create(AppUser user, string password)
        {
            return AsyncHelper.RunSync(() => CreateAsync(user, password));
        }

        public IdentityResult Create(AppUser user)
        {
            return AsyncHelper.RunSync(() => CreateAsync(user));
        }

        public ClaimsIdentity CreateIdentity(AppUser user, string authenticationType)
        {
            return AsyncHelper.RunSync(() => CreateIdentityAsync(user, authenticationType));
        }

        public IdentityResult Delete(AppUser user)
        {
            return AsyncHelper.RunSync(() => DeleteAsync(user));
        }

        public AppUser Find(string userName, string password)
        {
            return AsyncHelper.RunSync(() => FindAsync(userName, password));
        }

        public AppUser Find(UserLoginInfo login)
        {
            return AsyncHelper.RunSync(() => FindAsync(login));
        }

        public AppUser FindByEmail(string email)
        {
            return AsyncHelper.RunSync(() => FindByEmailAsync(email));
        }

        public AppUser FindById(int userId)
        {
            return AsyncHelper.RunSync(() => FindByIdAsync(userId));
        }

        public AppUser FindByName(string userName)
        {
            return AsyncHelper.RunSync(() => FindByNameAsync(userName));
        }

        public string GenerateChangePhoneNumberToken(int userId, string phoneNumber)
        {
            return AsyncHelper.RunSync(() => GenerateChangePhoneNumberTokenAsync(userId, phoneNumber));
        }

        public string GenerateEmailConfirmationToken(int userId)
        {
            return AsyncHelper.RunSync(() => GenerateEmailConfirmationTokenAsync(userId));
        }

        public string GeneratePasswordResetToken(int userId)
        {
            return AsyncHelper.RunSync(() => GeneratePasswordResetTokenAsync(userId));
        }

        public string GenerateTwoFactorToken(int userId, string twoFactorProvider)
        {
            return AsyncHelper.RunSync(() => GenerateTwoFactorTokenAsync(userId, twoFactorProvider));
        }

        public string GenerateUserToken(string purpose, int userId)
        {
            return AsyncHelper.RunSync(() => GenerateUserTokenAsync(purpose, userId));
        }

        public int GetAccessFailedCount(int userId)
        {
            return AsyncHelper.RunSync(() => GetAccessFailedCountAsync(userId));
        }

        public IList<Claim> GetClaims(int userId)
        {
            return AsyncHelper.RunSync(() => GetClaimsAsync(userId));
        }

        public string GetEmail(int userId)
        {
            return AsyncHelper.RunSync(() => GetEmailAsync(userId));
        }

        public bool GetLockoutEnabled(int userId)
        {
            return AsyncHelper.RunSync(() => GetLockoutEnabledAsync(userId));
        }

        public DateTimeOffset GetLockoutEndDate(int userId)
        {
            return AsyncHelper.RunSync(() => GetLockoutEndDateAsync(userId));
        }

        public IList<UserLoginInfo> GetLogins(int userId)
        {
            return AsyncHelper.RunSync(() => GetLoginsAsync(userId));
        }

        public string GetPhoneNumber(int userId)
        {
            return AsyncHelper.RunSync(() => GetPhoneNumberAsync(userId));
        }

        public IList<string> GetRoles(int userId)
        {
            return AsyncHelper.RunSync(() => GetRolesAsync(userId));
        }

        public string GetSecurityStamp(int userId)
        {
            return AsyncHelper.RunSync(() => GetSecurityStampAsync(userId));
        }

        public bool GetTwoFactorEnabled(int userId)
        {
            return AsyncHelper.RunSync(() => GetTwoFactorEnabledAsync(userId));
        }

        public IList<string> GetValidTwoFactorProviders(int userId)
        {
            return AsyncHelper.RunSync(() => GetValidTwoFactorProvidersAsync(userId));
        }

        public bool HasPassword(int userId)
        {
            return AsyncHelper.RunSync(() => HasPasswordAsync(userId));
        }

        public bool IsEmailConfirmed(int userId)
        {
            return AsyncHelper.RunSync(() => IsEmailConfirmedAsync(userId));
        }

        public bool IsInRole(int userId, string role)
        {
            return AsyncHelper.RunSync(() => IsInRoleAsync(userId, role));
        }

        public bool IsLockedOut(int userId)
        {
            return AsyncHelper.RunSync(() => IsLockedOutAsync(userId));
        }

        public bool IsPhoneNumberConfirmed(int userId)
        {
            return AsyncHelper.RunSync(() => IsPhoneNumberConfirmedAsync(userId));
        }

        public IdentityResult NotifyTwoFactorToken(int userId, string twoFactorProvider, string token)
        {
            return AsyncHelper.RunSync(() => NotifyTwoFactorTokenAsync(userId, twoFactorProvider, token));
        }

        public IdentityResult RemoveClaim(int userId, Claim claim)
        {
            return AsyncHelper.RunSync(() => RemoveClaimAsync(userId, claim));
        }

        public IdentityResult RemoveFromRole(int userId, string role)
        {
            return AsyncHelper.RunSync(() => RemoveFromRoleAsync(userId, role));
        }

        public IdentityResult RemoveFromRoles(int userId, params string[] roles)
        {
            return AsyncHelper.RunSync(() => RemoveFromRolesAsync(userId, roles));
        }

        public IdentityResult RemoveLogin(int userId, UserLoginInfo login)
        {
            return AsyncHelper.RunSync(() => RemoveLoginAsync(userId, login));
        }

        public IdentityResult RemovePassword(int userId)
        {
            return AsyncHelper.RunSync(() => RemovePasswordAsync(userId));
        }

        public IdentityResult ResetAccessFailedCount(int userId)
        {
            return AsyncHelper.RunSync(() => ResetAccessFailedCountAsync(userId));
        }

        public IdentityResult ResetPassword(int userId, string token, string newPassword)
        {
            return AsyncHelper.RunSync(() => ResetPasswordAsync(userId, token, newPassword));
        }

        public void SendEmail(int userId, string subject, string body)
        {
            AsyncHelper.RunSync(() => SendEmailAsync(userId, subject, body));
        }

        public void SendSms(int userId, string message)
        {
            AsyncHelper.RunSync(() => SendSmsAsync(userId, message));
        }

        public IdentityResult SetEmail(int userId, string email)
        {
            return AsyncHelper.RunSync(() => SetEmailAsync(userId, email));
        }

        public IdentityResult SetLockoutEnabled(int userId, bool enabled)
        {
            return AsyncHelper.RunSync(() => SetLockoutEnabledAsync(userId, enabled));
        }

        public IdentityResult SetLockoutEndDate(int userId, DateTimeOffset lockoutEnd)
        {
            return AsyncHelper.RunSync(() => SetLockoutEndDateAsync(userId, lockoutEnd));
        }

        public IdentityResult SetPhoneNumber(int userId, string phoneNumber)
        {
            return AsyncHelper.RunSync(() => SetPhoneNumberAsync(userId, phoneNumber));
        }

        public IdentityResult SetTwoFactorEnabled(int userId, bool enabled)
        {
            return AsyncHelper.RunSync(() => SetTwoFactorEnabledAsync(userId, enabled));
        }

        public IdentityResult Update(AppUser user)
        {
            return AsyncHelper.RunSync(() => UpdateAsync(user));
        }

        public IdentityResult UpdateSecurityStamp(int userId)
        {
            return AsyncHelper.RunSync(() => UpdateSecurityStampAsync(userId));
        }

        public bool VerifyChangePhoneNumberToken(int userId, string token, string phoneNumber)
        {
            return AsyncHelper.RunSync(() => VerifyChangePhoneNumberTokenAsync(userId, token, phoneNumber));
        }

        public bool VerifyTwoFactorToken(int userId, string twoFactorProvider, string token)
        {
            return AsyncHelper.RunSync(() => VerifyTwoFactorTokenAsync(userId, twoFactorProvider, token));
        }

        public bool VerifyUserToken(int userId, string purpose, string token)
        {
            return AsyncHelper.RunSync(() => VerifyUserTokenAsync(userId, purpose, token));
        }
    }


    public class MyDbContext : IdentityDbContext<
        AppUser,
        AppRole,
        int,
        AppUserLogin,
        AppUserRole,
        AppUserClaim>
    {
        // Other part of codes still same 
        // You don't need to add AppUser and AppRole 
        // since automatically added by inheriting form IdentityDbContext<AppUser>
        public MyDbContext()
        {
        }

        public MyDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public MyDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
        }

        public MyDbContext(string nameOrConnectionString, DbCompiledModel model) : base(nameOrConnectionString, model)
        {
        }
    }

}
