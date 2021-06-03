using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using LumenWorks.Framework.IO.Csv;
using SubsidizedDutyMeal.DAL;
using SubsidizedDutyMeal.Models;
using SubsidizedDutyMeal.Models.View_Model;

namespace SubsidizedDutyMeal.Controllers
{

    
    public class ScheduleController : Controller
    {
        private SDMContext db = new SDMContext();

        // GET: Schedule
        public ActionResult Index()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LogIn", "Account");
            }



            return View();
        }
        [HttpGet]
        public ActionResult Create()
        {

            return View();
        }
        [HttpGet]
        public ActionResult Edit(int?id)
        {

            var itemList = db.ScheduleDetails.Where(a => a.ScheduleHeaderID == id && a.Status == "Active")
                .Select(a => new {
                    a.ID,
                    a.EmployeeID,
                    a.EmployeeName,

                    a.Month,
                    a.Year,
                   
                    a.D1,
                    a.D2,
                    a.D3,
                    a.D4,
                    a.D5,
                    a.D6,
                    a.D7,
                    a.D8,
                    a.D9,
                    a.D10,
                    a.D11,
                    a.D12,
                    a.D13,
                    a.D14,
                    a.D15,
                    a.D16,
                    a.D17,
                    a.D18,
                    a.D19,
                    a.D20,
                    a.D21,
                    a.D22,
                    a.D23,
                    a.D24,
                    a.D25,
                    a.D26,
                    a.D27,
                    a.D28,
                    a.D29,
                    a.D30,
                    a.D31,
                    //a.Company,
                    //a.Department,
                    //a.ConcessionaireID,
                    a.ScheduleHeaderID,
                   
                }).OrderBy(a => a.ID);



            ViewBag.HeaderID = itemList.Select(i => i.ScheduleHeaderID).FirstOrDefault();


            var m = ToDataTable(itemList.ToList());
            DataTable dt = m;

            ViewBag.ScheduleHeaderID = id;
            return View(dt);
        }
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        [HttpPost]
        public JsonResult CheckIfExisting()
        {
            string message = "";
            string status = "";
            int userID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();

            try
            {
                if (TempData["csvData"] != null)
                {
                    TempData.Keep("csvData");
                    var dt = TempData["csvData"] as DataTable;

                    int nYear = Convert.ToInt32(dt.Rows[0][3].ToString());
                    int nMonth = Convert.ToInt32(dt.Rows[0][2].ToString());

                    int cnt = db.ScheduleHeaders.Where(a => a.Status == "Active").Where(a => a.Month == nMonth).Where(a => a.Year == nYear).Where(a => a.UserID == userID).Count();
                    if (cnt > 0)
                    {
                        message = "yes";
                    }
                    else
                    {
                        message = "no";
                    }
                    status = "success";
                }
            }
            catch (Exception e)
            {
                status = "fail";
                message = e.InnerException.Message;
                throw;
            }
            
            var result = new
            {
                status,
                message
            };

            return Json(result);
        }

        [HttpPost]
        public JsonResult PostSchedule()
        {
            int userID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();

            JsonArray result = new JsonArray();
            int refID = 0;
            int hdrCount = 0;
            if (TempData["csvData"] != null)
            {
                try
                {
                    TempData.Keep("csvData");
                    var dt = TempData["csvData"] as DataTable;


                    //no series
                    string lno = db.NoSeries.Where(i => i.Code == "UP").Select(n => n.LastNo).FirstOrDefault();
                    string s = lno;
                    int number = Convert.ToInt32(s);
                    number += 1;
                    string str = number.ToString("D7");

                    int nYear = Convert.ToInt32(dt.Rows[0][3].ToString());
                    int nMonth = Convert.ToInt32(dt.Rows[0][2].ToString());

                    var sheader = db.ScheduleHeaders.Where(a => a.Status == "Active").Where(a => a.Month == nMonth).Where(a => a.Year == nYear).Where(a => a.UserID == userID);
                    hdrCount = sheader.Count();

                    try
                    {
                        if (hdrCount == 0)
                        {
                            ScheduleHeader sh = new ScheduleHeader();
                            sh.Year = nYear;
                            sh.Month = nMonth;
                            sh.ReferenceNo = "UP-" + str;
                            sh.UserID = userID;
                            db.ScheduleHeaders.Add(sh);
                            db.SaveChanges();
                            refID = sh.ID;

                            NoSeries ns = db.NoSeries.SingleOrDefault(v => v.Code == "UP");
                            ns.LastNo = str;
                            db.SaveChanges();

                        }
                        else
                        {
                            refID = sheader.FirstOrDefault().ID;
                        }
                        
                       


                       
                    }
                    catch (Exception e)
                    {
                        result.message = e.InnerException.InnerException.Message;
                        result.status = "fail";
                        return Json(result);
                    }




                    //NoSeries ns = db.NoSeries.SingleOrDefault(v => v.Code == "UP");
                    //ns.LastNo = str;
                    //db.SaveChanges();
                    //end no series

                    if (hdrCount != 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string empid = dr[0].ToString();
                            var detail = db.ScheduleDetails
                                .Where(a => a.EmployeeID == empid)
                                .Where(a => a.ScheduleHeaderID == refID)
                                .FirstOrDefault();

                            if (detail != null)
                            {
                                detail.Status = "Deleted";
                                db.Entry(detail).State = EntityState.Modified;
                                db.SaveChanges();


                                Log lg = new Log();
                                lg.descriptions = "Delete schedule detail. Detail ID:" + detail.ID;
                                lg.otherinfo = "Replaced data. Uploaded new";
                                lg.user_id = userID;
                                db.Logs.Add(lg);
                                db.SaveChanges();
                            }
                            


                        }
                        
                    }

                        


                    ScheduleDetail sd = new ScheduleDetail();
                    foreach (DataRow dr in dt.Rows)
                    {
                        sd.EmployeeID = string.IsNullOrEmpty(dr[0].ToString()) ? "" : dr[0].ToString(); 
                        sd.EmployeeName = dr[1].ToString();
                        sd.Year = Convert.ToInt32(dr[3].ToString());
                        sd.Month = Convert.ToInt32(dr[2].ToString());
                        sd.D1 = string.IsNullOrEmpty(dr[4].ToString()) ? "" : dr[4].ToString();
                        sd.D2 = string.IsNullOrEmpty(dr[5].ToString()) ? "" : dr[5].ToString();
                        sd.D3 = string.IsNullOrEmpty(dr[6].ToString()) ? "" : dr[6].ToString();
                        sd.D4 = string.IsNullOrEmpty(dr[7].ToString()) ? "" : dr[7].ToString();
                        sd.D5 = string.IsNullOrEmpty(dr[8].ToString()) ? "" : dr[8].ToString();
                        sd.D6 = string.IsNullOrEmpty(dr[9].ToString()) ? "" : dr[9].ToString();
                        sd.D7 = string.IsNullOrEmpty(dr[10].ToString()) ? "" : dr[10].ToString();
                        sd.D8 = string.IsNullOrEmpty(dr[11].ToString()) ? "" : dr[11].ToString();
                        sd.D9 = string.IsNullOrEmpty(dr[12].ToString()) ? "" : dr[12].ToString();
                        sd.D10 = string.IsNullOrEmpty(dr[13].ToString()) ? "" : dr[13].ToString();
                        sd.D11 = string.IsNullOrEmpty(dr[14].ToString()) ? "" : dr[14].ToString();
                        sd.D12 = string.IsNullOrEmpty(dr[15].ToString()) ? "" : dr[15].ToString();
                        sd.D13 = string.IsNullOrEmpty(dr[16].ToString()) ? "" : dr[16].ToString();
                        sd.D14 = string.IsNullOrEmpty(dr[17].ToString()) ? "" : dr[17].ToString();
                        sd.D15 = string.IsNullOrEmpty(dr[18].ToString()) ? "" : dr[18].ToString();
                        sd.D16 = string.IsNullOrEmpty(dr[19].ToString()) ? "" : dr[19].ToString();
                        sd.D17 = string.IsNullOrEmpty(dr[20].ToString()) ? "" : dr[20].ToString();
                        sd.D18 = string.IsNullOrEmpty(dr[21].ToString()) ? "" : dr[21].ToString();
                        sd.D19 = string.IsNullOrEmpty(dr[22].ToString()) ? "" : dr[22].ToString();
                        sd.D20 = string.IsNullOrEmpty(dr[23].ToString()) ? "" : dr[23].ToString();
                        sd.D21 = string.IsNullOrEmpty(dr[24].ToString()) ? "" : dr[24].ToString();
                        sd.D22 = string.IsNullOrEmpty(dr[25].ToString()) ? "" : dr[25].ToString();
                        sd.D23 = string.IsNullOrEmpty(dr[26].ToString()) ? "" : dr[26].ToString();
                        sd.D24 = string.IsNullOrEmpty(dr[27].ToString()) ? "" : dr[27].ToString();
                        sd.D25 = string.IsNullOrEmpty(dr[28].ToString()) ? "" : dr[28].ToString();
                        sd.D26 = string.IsNullOrEmpty(dr[29].ToString()) ? "" : dr[29].ToString();
                        sd.D27 = string.IsNullOrEmpty(dr[30].ToString()) ? "" : dr[30].ToString();
                        sd.D28 = string.IsNullOrEmpty(dr[31].ToString()) ? "" : dr[31].ToString();
                        sd.D29 = string.IsNullOrEmpty(dr[32].ToString()) ? "" : dr[32].ToString();
                        sd.D30 = string.IsNullOrEmpty(dr[33].ToString()) ? "" : dr[33].ToString();
                        sd.D31 = string.IsNullOrEmpty(dr[34].ToString()) ? "" : dr[34].ToString();

                        sd.ScheduleHeaderID = refID;
                        db.ScheduleDetails.Add(sd);
                        db.SaveChanges();
                    }


                    Log log = new Log();
                    log.descriptions = "Create Schedule. Header ID:" + refID;
                    log.otherinfo = result.status + " " + result.message;
                    db.Logs.Add(log);
                    db.SaveChanges();

                    TempData.Remove("csvData");


                    result.status = "success";
                }
                catch (Exception ex)
                {
                    result.message = ex.Message;
                    result.status = "fail";

                }
                //finally {

                //    Log log = new Log();
                //    log.descriptions = "Create Schedule. Header ID:" + refID;
                //    log.otherinfo = result.status + " " + result.message;
                //    db.Logs.Add(log);
                //    db.SaveChanges();

                //    TempData.Remove("csvData");

                //}


            }





            return Json(result);
        }
        [HttpPost]
        public JsonResult UpdateSchedule(ScheduleDetail item)
        {

            JsonArray result = new JsonArray();
            if (item.ID == 0)
            {
                db.ScheduleDetails.Add(item);
                db.SaveChanges();

            }
            else
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
             
            }

            Log log = new Log();
            log.descriptions = "Modify Schedule. Header ID:" + item.ScheduleHeaderID;
            log.otherinfo = result.status + " " + result.message;
            db.Logs.Add(log);
            db.SaveChanges();

            result.status = "success";
            return Json(result);
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (upload != null && upload.ContentLength > 0)
                    {

                        if (upload.FileName.EndsWith(".csv"))
                        {
                            Stream stream = upload.InputStream;
                            DataTable csvTable = new DataTable();
                            using (CsvReader csvReader =
                                new CsvReader(new StreamReader(stream), true))
                            {

                                csvTable.Load(csvReader);
                                TempData.Remove("csvData");
                                TempData["csvData"] = csvTable;
                            }
                            return View("Create", csvTable);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "This file format is not supported");
                            return View("Create");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Please Upload Your file");
                    }
                }
                catch (Exception e)
                {

                    ModelState.AddModelError("File", e.InnerException);
                }
                
            }
            return View("Create");
        }
        public ActionResult getData(int draw, int start, int length, string strcode, string domain, int noCols)
        {

           
            int userID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();



            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;

            var v = db.ScheduleHeaders
                    //.Where(a=>a.UserID == userID)
                    .Where(b => b.Status == "Active")
                    .Select(b =>
                            new
                            {

                                b.ID,
                                b.ReferenceNo,
                                b.Month,
                                b.Year,
                                b.UserID,
                                Name = db.Users.Where(a => a.id == b.UserID).Select(a => a.Username).FirstOrDefault()
                                ,b.Status
                            }
                    );

            if (User.Identity.Name != "ADMIN")
            {
                v = v.Where(a => a.UserID == userID);
            }

            recordsTotal = v.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.ReferenceNo.ToUpper().Contains(searchValue.ToUpper())
                );
            }

            string strFilter = "";
            for (int i = 0; i < noCols; i++)
            {
                string colval = Request["columns[" + i + "][search][value]"];
                if (colval != "")
                {
                    string colSearch = Request["columns[" + i + "][data]"];
                    if (strFilter == "")
                    {
                        if (colval == "*")
                        {
                            strFilter = "(" + colSearch + " != " + "" + ")";
                        }
                        else
                        {
                            strFilter = colSearch + ".Contains(" + "\"" + colval + "\"" + ")";
                        }
                    }
                    else
                    {

                        strFilter = strFilter + " && " + colSearch + ".Contains(" + "\"" + colval + "\"" + ")";
                    }

                    if (strFilter != "")
                    {
                        v = v.Where(strFilter);
                    }
                }
            }



            recordsFiltered = v.Count();

            
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumn == "0")
                {
                    v = v.OrderByDescending(a => a.ID);
                }
                else
                {
                    string col = Request["columns[" + sortColumn + "][data]"];
                    v = v.OrderBy(col + " " + sortColumnDir);
                }

                
            }
            
            var x = v.ToList();
            if (pageSize < 0)
            {
                pageSize = recordsFiltered;
            }
            var lstData = v.Skip(skip).Take(pageSize).ToList();



            var model = new
            {
                draw = draw,
                recordsFiltered = recordsFiltered,
                recordsTotal = recordsTotal,
                data = lstData
            };
            return Json(model, JsonRequestBehavior.AllowGet);



        }
        [HttpPost]
        public ActionResult DeleteHeader(int? id)
        {
            JsonArray res = new JsonArray();

            try
            {


                Guid guid = Guid.NewGuid();


                ScheduleHeader detail = db.ScheduleHeaders.Find(id);
                detail.Status = "Deleted";
                detail.DeletedKey = guid;
                db.Entry(detail).State = EntityState.Modified;
                db.SaveChanges();
                res.status = "success";
            }
            catch (Exception ex)
            {
                res.message = ex.InnerException.InnerException.Message;
                res.status = "failed";
            }

            Log log = new Log();
            log.descriptions = "Delete Schedule. Header ID:" + id;
            log.otherinfo = res.status + " " + res.message;
            db.Logs.Add(log);
            db.SaveChanges();


            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteDetails(int? id)
        {
            JsonArray res = new JsonArray();

            try
            {
                ScheduleDetail detail = db.ScheduleDetails.Find(id);
                detail.Status = "Deleted";
                db.Entry(detail).State = EntityState.Modified;
                db.SaveChanges();
                res.status = "success";
            }
            catch (Exception ex)
            {
                res.message = ex.InnerException.InnerException.Message;
                res.status = "failed";
            }


            Log log = new Log();
            log.descriptions = "Delete Schedule Detail. Detail ID:" + id;
            log.otherinfo = res.status + " " + res.message;
            db.Logs.Add(log);
            db.SaveChanges();


            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}