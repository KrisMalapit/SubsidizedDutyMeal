using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace SubsidizedDutyMeal.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                        "~/Content/css/*.css"
                        , "~/Content/slick/*.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/scripts/bootstrap.min.js"
                        , "~/Content/scripts/jquery-3.3.1.js"
                        , "~/Content/scripts/jquery.dataTables.min.js"
                        , "~/Content/scripts/dataTables.bootstrap4.min.js"
                        , "~/Content/scripts/jquery-ui.js"
                        , "~/Content/scripts/jquery-1.9.1.min.js"
                        , "~/Content/scripts/bootstrap-datepicker.min.js"
                        , "~/Content/scripts/select2.min.js"
                        ,"~/Content/scripts/sweetalert.min.js"
                        , "~/Content/scripts/bootstrap-notify.min.js"
                        //, "~/Content/scripts/jquery-3.3.1.js"
                        , "~/Content/slick/slick.js"
                        ));

            
                

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            "~/Content/modernizr/modernizr-2.6.2.js"));
        }
    }
}