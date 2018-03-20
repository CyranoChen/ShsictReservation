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

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminlte").Include(
                "~/admin-lte/js/adminlte.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/icheck").Include(
                "~/Scripts/jquery.icheck.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/json2").Include(
                "~/Scripts/json2.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/bootstrap-theme.min.css",
                "~/admin-lte/css/AdminLTE.min.css",
                "~/admin-lte/css/skins/_all-skins.min.css",
                "~/Content/font-awesome.min.css",
                "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/iCheck/css").Include("~/Content/iCheck/all.css"));
        }
    }
}