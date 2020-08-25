using System;
using SeatDomain.Models;
using Exir.Framework.Common;
using FluentValidation;
using Exir.Framework.Security.DataSecurity.Support;
using Exir.Framework.Security.ObjectValidation.Support;


namespace SeatDomain.Validation
{
    public class NewsValidator : AzValidator<News>
    {
        public NewsValidator()
        {
            RuleSet(Constants.RuleSets.WithDetails, true, () =>
            {
                SecurityRuleFor().Allowed(AllFields(), Navigation(x => new { x.NewsCategory }));
            });

            RuleSet(Mode.Any, true, () =>
            {
                SecurityRuleFor().Allowed(AllFields());
                RuleFor(p => p.CategoryPk).NotNullOrEmptyIf(x => x.NewsCategory, Operators.Equal, null);
                RuleFor(p => p.Subject).NotNull().Length(1, 250);
                RuleFor(p => p.Abstract).NotNull().Length(1, 500);
                RuleFor(p => p.Audit_CreateDate).NotEmpty().NotNull();
                RuleFor(p => p.Audit_CreatorUserName).NotNull().Length(1, 50);
                RuleFor(p => p.Audit_CreatorIP).NotNull().Length(1, 50);
                RuleFor(p => p.Audit_LastModifyDate).NotEmpty().NotNull();
                RuleFor(p => p.Audit_LastModifierUserName).NotNull().Length(1, 50);
                RuleFor(p => p.Audit_LastModifierIP).NotNull().Length(1, 50);
                RuleFor(p => p.PageName).Length(0, 500);
                RuleFor(p => p.Metadata).Length(0, 500);
                RuleFor(p => p.SortNumber).GreaterThan(0);

            });
            setBusinessRules();
        }

        private void setBusinessRules()
        {
            RuleSet(Mode.OnInsert | Mode.OnDelete | Mode.OnUpdate, () =>
              {
                  SecurityRuleFor().OperationRequired("Nws.A");
              });
        }
    }

}
