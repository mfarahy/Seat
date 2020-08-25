using Exir.Framework.Uie;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace SeatWebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bool optimize;
            bool.TryParse(ConfigurationManager.AppSettings["optimize-bundles"], out optimize);
            BundleTable.EnableOptimizations = optimize;
            if (!bundles.Any(x => x.Path == "~/bundles/control-panel/js"))
            {
                var bundle = new ScriptBundle("~/bundles/control-panel/js");
                bundle.Orderer = new AsIsBundleOrderer();
                bundle
                    .Include("~/assets-web/panel/js/help-information.js")
                    .Include("~/assets-web/panel/js/short-link.js")
                    .Include("~/assets/plugins/ladda-bootstrap/spin.min.js")
                    .Include("~/assets/plugins/ladda-bootstrap/ladda.min.js")
                    .Include("~/assets-web/panel/js/website.js");
                bundles.Add(bundle);
            }
            if (!bundles.Any(x => x.Path == "~/bundles/control-panel/css"))
            {
                var bundle = new StyleBundle("~/bundles/control-panel/css");
                bundle.Orderer = new AsIsBundleOrderer();
                bundle.Include("~/assets-web/panel/css/control-panel.css");
                bundle.Include("~/assets/plugins/ladda-bootstrap/ladda-themeless.min.css");
                bundles.Add(bundle);
            }
            if (!bundles.Any(x => x.Path == "~/bundles/website/css"))
            {
                var bundle = new StyleBundle("~/bundles/website/css");
                bundle.Orderer = new AsIsBundleOrderer();
                bundle
                    .Include("~/assets-web/ui/css/style.css")
                    .Include("~/assets-web/ui/css/hover.css")
                    .Include("~/assets-web/ui/css/button.css")
                    .Include("~/assets-web/ui/css/form.css")
                    .Include("~/assets-web/ui/css/responsive.css")
                    ;
                bundles.Add(bundle);
            }
           
           
        }
    }
}
