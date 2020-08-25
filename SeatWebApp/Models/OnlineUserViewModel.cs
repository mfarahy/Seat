using System;
using Exir.Framework.Uie.Adapter;
using SeatDomain.Models;

namespace SeatWebApp.Models.Sso
{
    public class OnlineUserViewModel : EntityViewModel<OnlineUser>
    {
        public OnlineUserViewModel(object obj, string pk, Type pkType, string version)
            : base(obj, nameof(OnlineUser.SessionId), typeof(string), null, isKeyIdentity: true)
        {
        }
        public OnlineUserViewModel(OnlineUser obj)
            : base(obj)
        {
        }
    }
}