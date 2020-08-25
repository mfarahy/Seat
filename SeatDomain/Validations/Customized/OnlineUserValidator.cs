using Exir.Framework.Common;
using Exir.Framework.Security.DataSecurity.Support;
using SeatDomain.Models.Customized;

namespace SeatDomain.Validation
{
    public class OnlineUserValidator : AzValidator<OnlineUser>
    {
        public OnlineUserValidator()
        {
            setBusinessRules();
        }

        private void setBusinessRules()
        {
            RuleSet(Mode.OnBatchRead | Mode.OnSingleRead, () =>
            {
                SecurityRuleFor().OperationRequired("OnlnUsrs.A");
            });
        }
    }
}
