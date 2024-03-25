using System.Web.Optimization;

namespace AccountEx.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {


            bundles.Add(new StyleBundle("~/Content/Themes/cs/metronic").Include(



                 ///Metronic Thems

                 "~/Content/metronic/assets/global/css/components-rounded.css",
                "~/Content/metronic/assets/global/css/plugins.css",
                "~/Content/metronic/assets/layouts/layout/css/layout.css",
                "~/Content/metronic/assets/layouts/layout/css/themes/default.css",
                "~/Content/metronic/assets/global/plugins/jstree/dist/themes/default/style.min.css",
                "~/Content/metronic/assets/layouts/layout/css/custom.css"));

            //GLOBAL MANDATORY STYLES

            bundles.Add(new StyleBundle("~/Content/Themes/cs/bootstrap").Include(

                "~/Content/metronic/assets/global/plugins/bootstrap/css/bootstrap.min.css",
                "~/Content/metronic/assets/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css",
                "~/Content/metronic/assets/global/plugins/datatables/plugins/bootstrap/dataTables.bootstrap.css",
                "~/Content/metronic/assets/global/plugins/pace/themes/pace-theme-minimal.css"
                ));
            //PAGE LEVEL PLUGIN STYLES

            bundles.Add(new StyleBundle("~/Content/Themes/cs/plugin/select2").Include(
          "~/Content//metronic/assets/global/plugins/bootstrap-select/css/bootstrap-select.min.css",
                "~/Content//metronic/assets/global/plugins/select2/select2.css",
                "~/Content//metronic/assets/global/plugins/select2/select2-bootstrap.min.css"
                ));

            bundles.Add(new StyleBundle("~/Content/Themes/cs/plugin/uniform").Include(
        "~/Content/metronic/assets/global/plugins/uniform/css/uniform.default.css"
             ));

            bundles.Add(new StyleBundle("~/Content/Themes/cs/plugin/multiselect").Include(
            "~/Content//metronic/assets/global/plugins/jquery-multi-select/css/multi-select.css"
            ));

            bundles.Add(new StyleBundle("~/Content/Themes/cs/plugin/timepicker").Include(
              "~/Content//metronic/assets/global/plugins/bootstrap-timepicker/css/bootstrap-timepicker.min.css"
              ));

            bundles.Add(new StyleBundle("~/Content/Themes/cs/plugin/datepicker").Include(
               "~/Content//metronic/assets/global/plugins/bootstrap-datepicker/css/bootstrap-datepicker3.css",
                "~/Content//metronic/assets/global/plugins/bootstrap-daterangepicker/daterangepicker-bs3.css"
                ));
            bundles.Add(new StyleBundle("~/Content/Themes/cs/plugin/fileupload").Include(
               "~/Content//metronic/assets/global/plugins/jqvmap/jqvmap/jqvmap.css",
                "~/Content//metronic/assets/global/plugins/jquery-file-upload/css/jquery.fileupload.css",
                "~/Content//metronic/assets/global/plugins/jquery-file-upload/css/jquery.fileupload-ui.css"
                ));
            bundles.Add(new StyleBundle("~/Content/Themes/cs/plugin/fileuploadmaster").Include(
               "~/Content/metronic/assets/global/plugins/bootstrap-fileinput-master/bootstrap-fileinput-master/css/fileinput.css"
               ));

            bundles.Add(new StyleBundle("~/Content/Themes/cs/plugin/offline").Include(
           "~/Content/metronic/assets/global/plugins/offline-master/themes/offline-theme-slide.css",
                   "~/Content/metronic/assets/global/plugins/offline-master/themes/offline-language-english.css"
                   ));

            bundles.Add(new StyleBundle("~/Content/Themes/cs/plugin/jqueryui").Include(
             "~/Content/AutoComplete/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/Content/Themes/cs/plugin/toaster").Include(
             "~/Content/metronic/assets/global/plugins/bootstrap-toastr/toastr.min.css"));


            bundles.Add(new StyleBundle("~/Content/Themes/cs/plugin/timepicker").Include(
          "~/Content/metronic/assets/global/plugins/bootstrap-timepicker/css/bootstrap-timepicker.css"));


            bundles.Add(new StyleBundle("~/Content/Themes/cs/custom").Include(
               "~/Content/override.css"));

            bundles.Add(new StyleBundle("~/Content/Themes/cs/fontawsome").Include("~/Content/metronic/assets/global/plugins/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransform())
            .Include("~/Content/metronic/assets/global/plugins/simple-line-icons/simple-line-icons.css", new CssRewriteUrlTransform()));


            //Mandotary Plugin Scripts

            bundles.Add(new ScriptBundle("~/Content/scripts/mandatory").Include(
                  "~/Content/metronic/assets/global/plugins/jquery.min.js",
                 "~/Content/metronic/assets/global/plugins/bootstrap/js/bootstrap.min.js",
                  "~/Content/metronic/assets/global/plugins/jquery.blockui.min.js",
                "~/Content/metronic/assets/global/plugins/uniform/jquery.uniform.js",
                "~/Content/metronic/assets/global/plugins/datatables/dataTables.min.js",
                "~/Content/metronic/assets/global/plugins/datatables/plugins/bootstrap/dataTables.bootstrap.js",
                 "~/Content/metronic/assets/global/plugins/offline-master/js/offline.min.js"
                ));




            bundles.Add(new ScriptBundle("~/Content/scripts/plugin").Include(

                 //"~/Content/metronic/assets/global/plugins/jquery-3.1.1.js",

                 "~/Content/metronic/assets/global/plugins/js.cookie.min.js",
                 "~/Content/metronic/assets/global/plugins/bootstrap-hover-dropdown/bootstrap-hover-dropdown.min.js",
                 "~/Content/metronic/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                 "~/Content/metronic/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js",
                 "~/Content/metronic/assets/global/plugins/bootstrap-fileinput-master/bootstrap-fileinput-master/js/fileinput.js",
                  "~/Content/metronic/assets/global/plugins/bootstrap-daterangepicker/moment.min.js",
                   "~/Content/metronic/assets/global/plugins/pace/pace.min.js",
                    "~/Content/metronic/assets/global/plugins/bootstrap-toastr/toastr.min.js",


                //PAGE LEVEL PLUGINS
                "~/Content/metronic/assets/global/plugins/flot/jquery.flot.min.js",
                 "~/Content/metronic/assets/global/plugins/bootstrap-timepicker/js/bootstrap-timepicker.js",
                  "~/Content/metronic/assets/global/plugins/datatables/extensions/Responsive/js/dataTables.responsive.js",



                "~/Content/metronic/assets/global/plugins/flot/jquery.flot.resize.min.js",
                "~/Content/metronic/assets/global/plugins/flot/jquery.flot.categories.min.js",
                "~/Content/metronic/assets/global/plugins/jquery.pulsate.min.js",
                "~/Content/metronic/assets/global/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js",
                "~/Content/metronic/assets/global/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.min.js",

                "~/Content/metronic/assets/global/plugins/bootstrap-daterangepicker/daterangepicker.js",
                "~/Content/metronic/assets/global/plugins/jquery-multi-select/js/jquery.multi-select.js",
                "~/Content/metronic/assets/global/plugins/typeahead/handlebars.min.js",
                "~/Content/metronic/assets/global/plugins/typeahead/typeahead.bundle.min.js",
                "~/Content/metronic/assets/global/plugins/bootstrap-markdown/lib/markdown.js",
                "~/Content/metronic/assets/global/plugins/bootstrap-markdown/js/bootstrap-markdown.js",
                "~/Content/metronic/assets/global/plugins/jquery-inputmask/jquery.inputmask.bundle.min.js",
                "~/Content/metronic/assets/global/plugins/jquery-idle-timeout/jquery.idletimeout.js",
                "~/Content/metronic/assets/global/plugins/jquery-idle-timeout/jquery.idletimer.js",
                "~/Content/metronic/assets/global/plugins/StickyTableHeaders/js/jquery.stickytableheaders.js",
                "~/Content/metronic/assets/global/plugins/ckeditor/ckeditor.js",
                "~/Content/metronic/assets/global/plugins/bootstrap-maxlength/bootstrap-maxlength.js",
                "~/Content/metronic/assets/global/plugins/select2/select2.js",
                "~/Content/AutoComplete/jquery-ui.js",
                "~/Content/metronic/assets/global/scripts/app.js"));

            bundles.Add(new ScriptBundle("~/Content/scripts/timepicker").Include(
                 "~/Content/metronic/assets/global/plugins/bootstrap-timepicker/js/bootstrap-timepicker.js"
               ));


            bundles.Add(new ScriptBundle("~/Content/scripts/plugin/pagelevel").Include(
        "~/Content/metronic/assets/layouts/layout/scripts/layout.js",
                "~/Content/metronic/assets/layouts/global/scripts/quick-sidebar.js",
                "~/Content/metronic/assets/layouts/layout/scripts/demo.js",
                "~/Content/metronic/assets/pages/scripts/tasks.js",
                "~/Content/metronic/assets/pages/scripts/components-form-tools.js",
                "~/Content/metronic/assets/global/plugins/bootbox/bootbox.min.js",
                "~/Content/AppJS/Shared/autoNumeric.js",
                "~/Content/PlusasTab/emulatetab.joelpurra.js",
                "~/Content/PlusasTab/plusastab.js",
                "~/Content/AppJS/Global/ui-idletimeout.js",
                "~/Content/HotKey/jquery.hotkeys.js",
                "~/Content/FilterTable/filterTable.js"));

            //Table Export Plugin
            bundles.Add(new ScriptBundle("~/Content/scripts/plugin/tableexport").Include(
          "~/Content/metronic/assets/global/plugins/tableExport.jquery.plugin-master/libs/FileSaver/FileSaver.min.js",
          "~/Content/metronic/assets/global/plugins/tableExport.jquery.plugin-master/libs/js-xlsx/xlsx.core.min.js",
          "~/Content/metronic/assets/global/plugins/tableExport.jquery.plugin-master/tableExport.js",
          "~/Content/metronic/assets/global/plugins/tableExport.jquery.plugin-master/libs/jsPDF/jspdf.min.js",
          "~/Content/metronic/assets/global/plugins/tableExport.jquery.plugin-master/libs/jsPDF-AutoTable/jspdf.plugin.autotable.js"));


            //the jquery ui widget factory, can be omitted if jquery ui is already included
            bundles.Add(new ScriptBundle("~/content/scripts/plugin/fileupload").Include(
          "~/content/metronic/assets/global/plugins/jquery-file-upload/js/vendor/tmpl.min.js",
          "~/content/metronic/assets/global/plugins/jquery-file-upload/js/vendor/load-image.min.js",
           "~/content/metronic/assets/global/plugins/jquery-file-upload/js/jquery.fileupload.js",
          "~/content/metronic/assets/global/plugins/jquery-file-upload/js/jquery.fileupload-process.js",
          "~/content/metronic/assets/global/plugins/jquery-file-upload/js/jquery.fileupload-image.js",
          "~/content/metronic/assets/global/plugins/jquery-file-upload/js/jquery.fileupload-ui.js",
          "~/content/metronic/assets/pages/scripts/form-fileupload.js"));

            bundles.Add(new ScriptBundle("~/Content/scripts/plugin/linqjs").Include(
                  "~/Content/Linqjs/linq.js",
                  "~/Content/Linqjs/jquery.linq.js"));

            //PAGE LEVEL CUSTOM SCRIPTS
            bundles.Add(new ScriptBundle("~/Content/scripts/custom").Include(
              "~/Content/AppJS/Shared/Setting.js",
                   "~/Content/AppJS/Shared/Url.js",
                   "~/Content/AppJS/Shared/DataTable.js",
                    "~/Content/AppJS/Shared/Common.js",
                     "~/Content/AppJS/Shared/Enum.js"
                   ));

            //Login JS Bundle

            bundles.Add(new ScriptBundle("~/Content/scripts/login/plugin").Include(
        "~/Content/metronic/assets/global/plugins/jquery.min.js",
        "~/Content/metronic/assets/global/plugins/jquery.blockui.min.js",
                "~/Content/metronic/assets/global/plugins/bootstrap/js/bootstrap.min.js",
                "~/Content/metronic/assets/global/plugins/uniform/jquery.uniform.min.js",
                "~/Content/metronic/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js",
                "~/Content/metronic/assets/global/plugins/bootstrap-toastr/toastr.min.js",
                "~/Content/metronic/assets/global/plugins/select2/select2.min.js",
                "~/Content/metronic/assets/global/scripts/app.js"
               ));
            bundles.Add(new ScriptBundle("~/Content/scripts/login/custom").Include(
      "~/Content/AppJS/Shared/Setting.js",
              "~/Content/AppJS/Shared/Common.js",
              "~/Content/AppJS/Account/Login.js"
             ));





#if DEBUG
            bundles.IgnoreList.Clear();
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}