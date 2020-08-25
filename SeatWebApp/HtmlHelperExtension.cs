using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Exir.Framework.Uie.Contracts;
using Exir.Framework.Uie.Contracts.Support;
using Newtonsoft.Json;

namespace SeatWebApp
{
    public static class HtmlHelperExtension
    {
        public static MvcHtmlString ChangePercent(this HtmlHelper htmlHelper, double value)
        {
            return new MvcHtmlString(String.Format("<span class='{0}'>{1}</span>", value < 0 ? "mn" : "ps", value.ToString("+#.##;-#.##;0.##")));
        }
        public static MvcHtmlString NumberDecorator(this HtmlHelper htmlHelper, double value)
        {
            return new MvcHtmlString(String.Format("<span class='number'>{0}</span>", value.ToString("+#,##0.##;-#,##0.##;0")));
        }

    }
}
