using Exir.Framework.Uie.Adapter;
using System;
using Exir.Framework.Uie.Bocrud;
using Exir.Framework.Common;
using SeatDomain.Models;
using SeatDomain.Services;

namespace SeatWebApp.Models
{
    public partial class UserProfileViewModel : EntityViewModel<UserProfile>
    {
        public UserProfileViewModel(object obj, string pk, Type pkType, string version)
            : base(obj, nameof(UserProfile.ProfilePK), typeof(int), null, isKeyIdentity: true)
        {
        }
        public UserProfileViewModel(UserProfile obj)
            : base(obj)
        {
        }

        protected override void OnShowContent(ShowContentEventArgs args)
        {
            var profileService = StaticServiceFactory.Create<IUserProfileService>();

            var profile = profileService.GetCurrentProfile();

            Container = profile;

            base.OnShowContent(args);
        }


    }

}
