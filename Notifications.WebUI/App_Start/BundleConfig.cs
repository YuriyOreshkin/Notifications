using System.Web;
using System.Web.Optimization;

namespace Notifications.WebUI
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            //kendo scripts 
            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                      "~/Scripts/kendo/2019.2.514/jquery.min.js",
                      "~/Scripts/kendo/2019.2.514/jszip.min.js",
                      "~/Scripts/kendo/2019.2.514/kendo.all.min.js",
                      "~/Scripts/kendo/2019.2.514/kendo.aspnetmvc.min.js",
                      "~/Scripts/kendo/2019.2.514/cultures/kendo.culture.ru-RU.min.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                       "~/Content/kendo/2019.2.514/kendo.common.min.css",
                       "~/Content/kendo/2019.2.514/kendo.dataviz.min.css",
                        "~/Content/kendo/2019.2.514/kendo.material.min.css",
                 "~/Content/kendo/2019.2.514/kendo.dataviz.material.min.css"));


        }
    }
}
