using Exir.Framework.Common;
using Exir.Framework.Uie.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using Exir.Framework.Uie.Bocrud;
using SeatDomain.Models;

namespace SeatWebApp.Models
{
    public partial class HelpInformationViewModel : EntityViewModel<HelpInformation>
    {
        public HelpInformationViewModel(object obj, string pk, Type pkType, string version)
            : base(obj, nameof(HelpInformation.HelpInfoPK), typeof(int), null, true)
        {
        }
        public HelpInformationViewModel(HelpInformation obj)
            : base(obj)
        {
        }

        public List<string> RoleNames { get; set; } = new List<string>();

        public Dictionary<string, string> GetFileNames(bool loadAll, string formname)
        {
            var modelProvider = Exir.Framework.Uie.Bocrud.BocrudControlSettings.Instance.GetService<Exir.Framework.Uie.Contracts.IModelProvider>();

            if (!loadAll)
            {
                if (!String.IsNullOrEmpty(formname))
                {
                    var model = modelProvider.Load(formname);
                    return new Dictionary<string, string>() { { formname, model.Caption } };
                }
                else
                    return new Dictionary<string, string>();
            }


            var models = modelProvider.LoadAll().OrderByDescending(x => x.Caption).Select(x => new KeyValuePair<string, string>(x.Name, x.Caption));

            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var model in models)
                if (!result.ContainsKey(model.Key))
                    result.Add(model.Key, model.Value);
            return result;
        }

        public Dictionary<string, string> GetFields(string formName)
        {
            if (String.IsNullOrEmpty(formName)) return new Dictionary<string, string>();

            var modelProvider = Exir.Framework.Uie.Bocrud.BocrudControlSettings.Instance.GetService<Exir.Framework.Uie.Contracts.IModelProvider>();
            var form = modelProvider.Load(formName);
            var controls = form.AllControls
                .Where(x => !String.IsNullOrEmpty(x.Caption))
                .OrderByDescending(x => x.Caption)
                .Select(x => new KeyValuePair<string, string>(x.Name, x.Caption));

            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var control in controls)
                if (!result.ContainsKey(control.Key))
                    result.Add(control.Key, control.Value);

            if (form.ToolbarCommands != null)
                foreach (var b in form.ToolbarCommands)
                    result.Add(":" + b.Name, "> دکمه " + b.Text);

            result.AddRange(form.AllControls.Select(x => x.Group)
                .Where(x => x != null && (!String.IsNullOrEmpty(x.Title) || !String.IsNullOrEmpty(x.Name)))
                .Distinct()
                .Select(x => new KeyValuePair<string, string>("::GROUP" + (String.IsNullOrEmpty(x.Name) ? x.Title : x.Name), "> بخش " + (String.IsNullOrEmpty(x.Title) ? x.Name : x.Title)))
                .Distinct());

            result.Add(":TOP", "> بالای صفحه");
            result.Add(":DOWN", "> پایین صفحه");
            result.Add("::NEW", "> بالای فرم جدید");
            result.Add("::EDIT", "> بالای فرم ویرایش");
            result.Add("::VIEW", "> بالای فرم مشاهده");

            return result;
        }

        protected override void OnPreSave(DispatcherEventArgs args)
        {
            if (RoleNames != null)
                Container.RoleName = String.Join(";", RoleNames);
        }

        protected override void OnShowContent(ShowContentEventArgs args)
        {
            if (Container.RoleName != null)
            {
                var roles = Container.RoleName.Split(';').ToList();
                RoleNames = new List<string>();
                foreach (var role in roles)
                    RoleNames.Add(role);
            }

        }
    }

}
