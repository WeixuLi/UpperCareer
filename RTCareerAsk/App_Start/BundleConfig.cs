using System.Web;
using System.Web.Optimization;

namespace RTCareerAsk
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/base").Include(
                        "~/Scripts/bootstrap*",
                        "~/Scripts/Upper/upper-base*",
                        "~/Scripts/Upper/new-post.js"));

            bundles.Add(new ScriptBundle("~/bundles/content-list").Include(
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/upper-tabs.js",
                        "~/Scripts/Upper/question-list.js",
                        "~/Scripts/Upper/upper-shorten.js",
                        "~/Scripts/Upper/upper-scrollpaging.js"));

            bundles.Add(new ScriptBundle("~/bundles/feeds").Include(
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/upper-shorten.js",
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/feeds.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                        "~/Scripts/Upper/login.js")); ;


            bundles.Add(new ScriptBundle("~/bundles/question").Include(
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/upper-shorten.js",
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/upper-confirmdialog.js",
                        "~/Scripts/Upper/question-answer-edit.js"));

            bundles.Add(new ScriptBundle("~/bundles/answer").Include(
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/upper-confirmdialog.js",
                        "~/Scripts/Upper/question-answer-edit.js"));

            bundles.Add(new ScriptBundle("~/bundles/user").Include(
                        "~/Scripts/Upper/user-page.js",
                        "~/Scripts/Upper/upper-tabs.js",
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/upper-shorten.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/search").Include(
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/search-result*"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/message").Include(
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/upper-tabs.js",
                        "~/Scripts/Upper/upper-confirmdialog.js",
                        "~/Scripts/Upper/notification-index.js",
                        "~/Scripts/Upper/message-index.js"));

            bundles.Add(new ScriptBundle("~/bundles/notification").Include(
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/upper-tabs.js",
                        "~/Scripts/Upper/notification-all.js"));

            bundles.Add(new ScriptBundle("~/bundles/manage").Include(
                        "~/Scripts/Upper/user-manage.js",
                        "~/Scripts/cropper*",
                        "~/Scripts/canvas-to-blob*"));

            bundles.Add(new ScriptBundle("~/bundles/article").Include(
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/upper-scrollpaging.js",
                        "~/Scripts/Upper/upper-article-detail.js"));

            bundles.Add(new ScriptBundle("~/bundles/article-list").Include(
                        "~/Scripts/Upper/upper-scrollpaging.js"));

            bundles.Add(new ScriptBundle("~/bundles/compose").Include(
                        "~/Scripts/Upper/fix-image-display.js",
                        "~/Scripts/Upper/article-compose.js",
                        "~/Scripts/canvas-to-blob*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.upper.css",
                        "~/Content/Upper.css",
                        "~/Content/preloader.css"));

            bundles.Add(new StyleBundle("~/Content/cropper").Include(
                        "~/Content/cropper*"));


            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}