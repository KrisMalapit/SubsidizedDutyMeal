using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using SubsidizedDutyMeal.DAL;

namespace SubsidizedDutyMeal.Controllers
{
    
    public class HomeController : Controller
    {
        private SDMContext db = new SDMContext();
        //
        // GET: /Home/

        public ActionResult Index()
        {



            //if (User.Identity.IsAuthenticated)
            //{
            //    ViewBag.UserName = "Kris";
            //}

            return View();
        }
        public ActionResult DailySummary(DateTime TransDate)
        {


            int ConcessionaireID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();

            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;
            

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\rptDailySummary.rdlc";
            dt = new ReportController().PrintDailySummary(ConcessionaireID, TransDate);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("DailySummary", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult DailySummaryExcel(DateTime TransDate)
        {


            int ConcessionaireID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();

            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;


            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\rptDailySummary.rdlc";
            dt = new ReportController().PrintDailySummary(ConcessionaireID, TransDate);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("DailySummary", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult MealSummary(DateTime FromDate, DateTime ToDate,string Status,string CompanyCode)
        {
            int ConcessionaireID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();
       
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;


            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\rptMealSummary.rdlc";
            dt = new ReportController().PrintMealSummary(ConcessionaireID, FromDate,ToDate,Status, CompanyCode);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("MealSummary", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            return new FileContentResult(bytes, mimeType);


        }
        public ActionResult MealSummaryExcel(DateTime FromDate, DateTime ToDate, string Status, string CompanyCode)
        {
            int ConcessionaireID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;


            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\rptMealSummary.rdlc";
            dt = new ReportController().PrintMealSummary(ConcessionaireID, FromDate, ToDate, Status, CompanyCode);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("MealSummary", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);


        }
        public ActionResult MealStatement(DateTime FromDate, DateTime ToDate, string CompanyCode,string rpttype)
        {
            int ConcessionaireID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();

            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;


            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\rptStatement.rdlc";
            dt = new ReportController().PrintStatement(ConcessionaireID, FromDate, ToDate, CompanyCode);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("MealStatement", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render(rpttype, null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            return new FileContentResult(bytes, mimeType);


        }
        
        public ActionResult AvailedMeals(DateTime FromDate, DateTime ToDate, string rpttype)
        {
            int ConcessionaireID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();

            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;


            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\rptAvailedMeal.rdlc";
            dt = new ReportController().PrintAvailedMeals(ConcessionaireID, FromDate, ToDate);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("AvailedMeals", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render(rpttype, null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            return new FileContentResult(bytes, mimeType);


        }
        public ActionResult FeedBackSummary(DateTime FromDate, DateTime ToDate, string rpttype)
        {
            

            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;


            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\rptFeedBack.rdlc";
            dt = new ReportController().PrintFeedBackSummary(FromDate, ToDate);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("FeedBack", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render(rpttype, null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            return new FileContentResult(bytes, mimeType);


        }
        public ActionResult ConcessionaireSummary(DateTime FromDate, DateTime ToDate, string rpttype)
        {


            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;


            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\rptConcessionaire.rdlc";
            dt = new ReportController().PrintConcessionaireSummary(FromDate, ToDate);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("Concessionaire", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render(rpttype, null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            return new FileContentResult(bytes, mimeType);


        }


    }
}