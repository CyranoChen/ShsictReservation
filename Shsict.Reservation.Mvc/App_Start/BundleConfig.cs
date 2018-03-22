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

            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                "~/Scripts/datepicker/bootstrap-datepicker.min.js",
                "~/Scripts/datepicker/locales/bootstrap-datepicker.zh-CN.js"));

            bundles.Add(new ScriptBundle("~/bundles/timepicker").Include(
                "~/Scripts/timepicker/bootstrap-timepicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                "~/Scripts/datatables/jquery.dataTables.min.js",
                "~/Scripts/datatables/dataTables.bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/bootstrap-theme.min.css",
                "~/admin-lte/css/AdminLTE.min.css",
                "~/admin-lte/css/skins/_all-skins.min.css",
                "~/Content/font-awesome.min.css",
                "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/iCheck/css").Include("~/Content/iCheck/all.min.css"));

            bundles.Add(new StyleBundle("~/Content/datepicker/css").Include(
                "~/Scripts/datepicker/datepicker3.min.css"));

            bundles.Add(new StyleBundle("~/Content/timepicker/css").Include(
                "~/Scripts/timepicker/bootstrap-timepicker.min.css"));

            bundles.Add(new ScriptBundle("~/Content/datatables/css").Include(
                "~/Scripts/datatables/dataTables.bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/FontAwesome5/css").Include("~/Content/fontawesome-all.min.css"));
        }
    }
}