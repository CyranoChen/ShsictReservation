using System.Web.Optimization;

namespace Shsict.Reservation.Mvc
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminlte").Include(
                "~/Scripts/app.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/icheck").Include(
                "~/Scripts/jquery.icheck.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/bootstrap-theme.min.css",
                "~/Content/adminlte.min.css",
                "~/Content/skins/_all-skins.min.css",
                "~/Content/font-awesome.min.css",
                "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/iCheck/css").Include("~/Content/iCheck/all.css"));
        }
    }
}