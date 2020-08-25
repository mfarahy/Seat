using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
namespace SeatWebApp.Security
{
    public interface IUserManager<TUser, TKey> : IDisposable
          where TUser : class, IUser<TKey>
         where TKey : IEquatable<TKey>
    {
        IIdentityValidator<TUser> UserValidator { get; set; }
        IIdentityValidator<string> PasswordValidator { get; set; }
        IClaimsIdentityFactory<TUser, TKey> ClaimsIdentityFactory { get; set; }
        IIdentityMessageService EmailService { get; set; }
        IIdentityMessageService SmsService { get; set; }
        IUserTokenProvider<TUser, TKey> UserTokenProvider { get; set; }
        bool UserLockoutEnabledByDefault { get; set; }
        int MaxFailedAccessAttemptsBeforeLockout { get; set; }
        TimeSpan DefaultAccountLockoutTimeSpan { get; set; }
        bool SupportsUserTwoFactor { get; }
        bool SupportsUserPassword { get; }
        bool SupportsUserSecurityStamp { get; }
        bool SupportsUserRole { get; }
        bool SupportsUserLogin { get; }
        bool SupportsUserEmail { get; }
        bool SupportsUserPhoneNumber { get; }
        bool SupportsUserClaim { get; }
        bool SupportsUserLockout { get; }
        bool SupportsQueryableUsers { get; }
        IPasswordHasher PasswordHasher { get; set; }
        IQueryable<TUser> Users { get; }
        IDictionary<string, IUserTokenProvider<TUser, TKey>> TwoFactorProviders { get; }
        Task<IdentityResult> AccessFailedAsync(TKey userId);
        Task<IdentityResult> AddClaimAsync(TKey userId, Claim claim);
        Task<IdentityResult> AddLoginAsync(TKey userId, UserLoginInfo login);
        Task<IdentityResult> AddPasswordAsync(TKey userId, string password);
        Task<IdentityResult> AddToRoleAsync(TKey userId, string role);
        Task<IdentityResult> AddToRolesAsync(TKey userId, params string[] roles);
        Task<IdentityResult> ChangePasswordAsync(TKey userId, string currentPassword, string newPassword);
        Task<IdentityResult> ChangePhoneNumberAsync(TKey userId, string phoneNumber, string token);
        Task<bool> CheckPasswordAsync(TUser user, string password);
        Task<IdentityResult> ConfirmEmailAsync(TKey userId, string token);
        Task<IdentityResult> CreateAsync(TUser user, string password);
        Task<IdentityResult> CreateAsync(TUser user);
        Task<ClaimsIdentity> CreateIdentityAsync(TUser user, string authenticationType);
        Task<IdentityResult> DeleteAsync(TUser user);
        Task<TUser> FindAsync(string userName, string password);
        Task<TUser> FindAsync(UserLoginInfo login);
        Task<TUser> FindByEmailAsync(string email);
        Task<TUser> FindByIdAsync(TKey userId);
        Task<TUser> FindByNameAsync(string userName);
        Task<string> GenerateChangePhoneNumberTokenAsync(TKey userId, string phoneNumber);
        Task<string> GenerateEmailConfirmationTokenAsync(TKey userId);
        Task<string> GeneratePasswordResetTokenAsync(TKey userId);
        Task<string> GenerateTwoFactorTokenAsync(TKey userId, string twoFactorProvider);
        Task<string> GenerateUserTokenAsync(string purpose, TKey userId);
        Task<int> GetAccessFailedCountAsync(TKey userId);
        Task<IList<Claim>> GetClaimsAsync(TKey userId);
        Task<string> GetEmailAsync(TKey userId);
        Task<bool> GetLockoutEnabledAsync(TKey userId);
        Task<DateTimeOffset> GetLockoutEndDateAsync(TKey userId);
        Task<IList<UserLoginInfo>> GetLoginsAsync(TKey userId);
        Task<string> GetPhoneNumberAsync(TKey userId);
        Task<IList<string>> GetRolesAsync(TKey userId);
        Task<string> GetSecurityStampAsync(TKey userId);
        Task<bool> GetTwoFactorEnabledAsync(TKey userId);
        Task<IList<string>> GetValidTwoFactorProvidersAsync(TKey userId);
        Task<bool> HasPasswordAsync(TKey userId);
        Task<bool> IsEmailConfirmedAsync(TKey userId);
        Task<bool> IsInRoleAsync(TKey userId, string role);
        Task<bool> IsLockedOutAsync(TKey userId);
        Task<bool> IsPhoneNumberConfirmedAsync(TKey userId);
        Task<IdentityResult> NotifyTwoFactorTokenAsync(TKey userId, string twoFactorProvider, string token);
        void RegisterTwoFactorProvider(string twoFactorProvider, IUserTokenProvider<TUser, TKey> provider);
        Task<IdentityResult> RemoveClaimAsync(TKey userId, Claim claim);
        Task<IdentityResult> RemoveFromRoleAsync(TKey userId, string role);
        Task<IdentityResult> RemoveFromRolesAsync(TKey userId, params string[] roles);
        Task<IdentityResult> RemoveLoginAsync(TKey userId, UserLoginInfo login);
        Task<IdentityResult> RemovePasswordAsync(TKey userId);
        Task<IdentityResult> ResetAccessFailedCountAsync(TKey userId);
        Task<IdentityResult> ResetPasswordAsync(TKey userId, string token, string newPassword);
        Task SendEmailAsync(TKey userId, string subject, string body);
        Task SendSmsAsync(TKey userId, string message);
        Task<IdentityResult> SetEmailAsync(TKey userId, string email);
        Task<IdentityResult> SetLockoutEnabledAsync(TKey userId, bool enabled);
        Task<IdentityResult> SetLockoutEndDateAsync(TKey userId, DateTimeOffset lockoutEnd);
        Task<IdentityResult> SetPhoneNumberAsync(TKey userId, string phoneNumber);
        Task<IdentityResult> SetTwoFactorEnabledAsync(TKey userId, bool enabled);
        Task<IdentityResult> UpdateAsync(TUser user);
        Task<IdentityResult> UpdateSecurityStampAsync(TKey userId);
        Task<bool> VerifyChangePhoneNumberTokenAsync(TKey userId, string token, string phoneNumber);
        Task<bool> VerifyTwoFactorTokenAsync(TKey userId, string twoFactorProvider, string token);
        Task<bool> VerifyUserTokenAsync(TKey userId, string purpose, string token);

        /*  SYNC  */

        IdentityResult AccessFailed(TKey userId);
        IdentityResult AddClaim(TKey userId, Claim claim);
        IdentityResult AddLogin(TKey userId, UserLoginInfo login);
        IdentityResult AddPassword(TKey userId, string password);
        IdentityResult AddToRole(TKey userId, string role);
        IdentityResult AddToRoles(TKey userId, params string[] roles);
        IdentityResult ChangePassword(TKey userId, string currentPassword, string newPassword);
        IdentityResult ChangePhoneNumber(TKey userId, string phoneNumber, string token);
        bool CheckPassword(TUser user, string password);
        IdentityResult ConfirmEmail(TKey userId, string token);
        IdentityResult Create(TUser user, string password);
        IdentityResult Create(TUser user);
        ClaimsIdentity CreateIdentity(TUser user, string authenticationType);
        IdentityResult Delete(TUser user);
        TUser Find(string userName, string password);
        TUser Find(UserLoginInfo login);
        TUser FindByEmail(string email);
        TUser FindById(TKey userId);
        TUser FindByName(string userName);
        string GenerateChangePhoneNumberToken(TKey userId, string phoneNumber);
        string GenerateEmailConfirmationToken(TKey userId);
        string GeneratePasswordResetToken(TKey userId);
        string GenerateTwoFactorToken(TKey userId, string twoFactorProvider);
        string GenerateUserToken(string purpose, TKey userId);
        int GetAccessFailedCount(TKey userId);
        IList<Claim> GetClaims(TKey userId);
        string GetEmail(TKey userId);
        bool GetLockoutEnabled(TKey userId);
        DateTimeOffset GetLockoutEndDate(TKey userId);
        IList<UserLoginInfo> GetLogins(TKey userId);
        string GetPhoneNumber(TKey userId);
        IList<string> GetRoles(TKey userId);
        string GetSecurityStamp(TKey userId);
        bool GetTwoFactorEnabled(TKey userId);
        IList<string> GetValidTwoFactorProviders(TKey userId);
        bool HasPassword(TKey userId);
        bool IsEmailConfirmed(TKey userId);
        bool IsInRole(TKey userId, string role);
        bool IsLockedOut(TKey userId);
        bool IsPhoneNumberConfirmed(TKey userId);
        IdentityResult NotifyTwoFactorToken(TKey userId, string twoFactorProvider, string token);
        IdentityResult RemoveClaim(TKey userId, Claim claim);
        IdentityResult RemoveFromRole(TKey userId, string role);
        IdentityResult RemoveFromRoles(TKey userId, params string[] roles);
        IdentityResult RemoveLogin(TKey userId, UserLoginInfo login);
        IdentityResult RemovePassword(TKey userId);
        IdentityResult ResetAccessFailedCount(TKey userId);
        IdentityResult ResetPassword(TKey userId, string token, string newPassword);
        void SendEmail(TKey userId, string subject, string body);
        void SendSms(TKey userId, string message);
        IdentityResult SetEmail(TKey userId, string email);
        IdentityResult SetLockoutEnabled(TKey userId, bool enabled);
        IdentityResult SetLockoutEndDate(TKey userId, DateTimeOffset lockoutEnd);
        IdentityResult SetPhoneNumber(TKey userId, string phoneNumber);
        IdentityResult SetTwoFactorEnabled(TKey userId, bool enabled);
        IdentityResult Update(TUser user);
        IdentityResult UpdateSecurityStamp(TKey userId);
        bool VerifyChangePhoneNumberToken(TKey userId, string token, string phoneNumber);
        bool VerifyTwoFactorToken(TKey userId, string twoFactorProvider, string token);
        bool VerifyUserToken(TKey userId, string purpose, string token);
    }
}