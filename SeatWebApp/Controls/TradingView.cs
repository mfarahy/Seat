using Exir.Framework.Uie.Bocrud;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace SeatWebApp.Controls
{
    public class TradingView : FormControl
    {
        public override string RenderHtml()
        {
            StringWriter sw = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(sw);
            writer.AddAttribute("class", "tradingview-widget-container");
            writer.RenderBeginTag("div");
            string uid = UniqueId;
            writer.AddAttribute("id", uid);
            writer.RenderBeginTag("div");
            writer.RenderEndTag();

            var options = new Options
            {
                width = "100%",
                height = 550,
                symbol = "BITSTAMP:BTCUSD",
                interval = "D",
                timezone = "Etc/UTC",
                theme = "Light",
                style = "1",
                locale = "en",
                toolbar_bg = "#f1f3f6",
                enable_publishing = false,
                withdateranges = true,
                hide_side_toolbar = false,
                allow_symbol_change = true,
                show_popup_button = true,
                popup_width = "1000",
                popup_height = "650",
                container_id = uid
            };
            string code = String.Format("$.bocrud.tv({0});", JsonConvert.SerializeObject(options));

            var apply = RegisterTryLoadFileScripts("/assets-web/js/apply.js", String.Format("function(){{ {0} }}", code), RegistrationType.Function);
            var script = RegisterTryLoadFileScripts("/assets-web/js/tv.js", apply, RegistrationType.ScriptTag);
            writer.Write(script);

            writer.RenderEndTag();
            return sw.ToString();
        }


        public class Options
        {
            public string width { get; set; }
            public int height { get; set; }
            public string symbol { get; set; }
            public string interval { get; set; }
            public string timezone { get; set; }
            public string theme { get; set; }
            public string style { get; set; }
            public string locale { get; set; }
            public string toolbar_bg { get; set; }
            public bool enable_publishing { get; set; }
            public bool withdateranges { get; set; }
            public bool hide_side_toolbar { get; set; }
            public bool allow_symbol_change { get; set; }
            public bool show_popup_button { get; set; }
            public string popup_width { get; set; }
            public string popup_height { get; set; }
            public string container_id { get; set; }
        }
    }
}