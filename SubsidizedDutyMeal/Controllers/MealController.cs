using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using SubsidizedDutyMeal.DAL;
using SubsidizedDutyMeal.Models;
using SubsidizedDutyMeal.Models.View_Model;

namespace SubsidizedDutyMeal.Controllers
{
    public class MealController : Controller
    {
        private SDMContext db = new SDMContext();
        // GET: Meal
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Details(int? id)
        {
            DateTime dt = new DateTime();
            if (id != 0)
            {
                dt = db.MealHeaders.Where(a => a.ID == id).Select(a => a.Date).FirstOrDefault();
            }
            else
            {
                dt = DateTime.Now;
            }






            ViewBag.Header = id;
            ViewBag.Date = dt.ToShortDateString();
            return View();
        }

        public ActionResult getData(int draw, int start, int length, string strcode, string domain, int noCols)
        {
            int ConcessionaireID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();



            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;

            var v = db.MealHeaders
                    .Where(b => b.Status == "Active")
                    .Where(b => b.ConcessionaireID == ConcessionaireID)

                    .Select(b =>
                            new
                            {

                                b.ID,
                                b.Date

                            }
                    );

            recordsTotal = v.Count();

            //if (!string.IsNullOrEmpty(searchValue))
            //{
            //    v = v.Where("Date = @0", dval); 
            //}
            DateTime dval = new DateTime();
            Boolean hasDateCreated = false;
            string strFilter = "";
            for (int i = 0; i < noCols; i++)
            {
                string colval = Request["columns[" + i + "][search][value]"];
                if (colval != "")
                {
                    string colSearch = Request["columns[" + i + "][data]"];


                    if (colSearch == "Date")
                    {
                        dval = DateTime.Parse(colval);
                        hasDateCreated = true;
                    }
                    else
                    {
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
                    }
                    if (strFilter != "")
                    {
                        v = v.Where(strFilter);
                    }
                }
            }

            if (hasDateCreated)
            {
                v = v.Where("Date = @0", dval);
            }

            recordsFiltered = v.Count();


            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                string col = Request["columns[" + sortColumn + "][data]"];
                v = v.OrderBy(col + " " + sortColumnDir);
            }

            var x = v.ToList();
            if (pageSize < 0)
            {
                pageSize = recordsFiltered;
            }
            var lstData = v.OrderByDescending(a => a.Date).Skip(skip).Take(pageSize).ToList();



            var model = new
            {
                draw = draw,
                recordsFiltered = recordsFiltered,
                recordsTotal = recordsTotal,
                data = lstData
            };
            return Json(model, JsonRequestBehavior.AllowGet);



        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult ShowDetails(int? id)
        {

            JsonArray result = new JsonArray();
            var details = db.MealDetails.Find(id);
            result.param1 = details.Name;
            result.param2 = details.MealType;
            result.param3 = details.Descriptions;
            result.num1 = details.NoServings;
            result.status = "success";
            return Json(result);
        }
        public ActionResult UpdateDetails(MealDetails dt)
        {
            int vMealHeaderID = 0;
            JsonArray result = new JsonArray();
            try
            {
                vMealHeaderID = db.MealDetails.Where(a => a.ID == dt.ID).Select(a => a.MealHeaderID).FirstOrDefault();
                dt.MealHeaderID = vMealHeaderID;


                db.Entry(dt).State = EntityState.Modified;
                db.SaveChanges();
                result.status = "success";

            }
            catch (Exception ex)
            {
                result.message = ex.Message;
                result.status = "fail";
                throw;
            }

            Log log = new Log();
            log.descriptions = "Modify all Meal Detail. Header ID:" + vMealHeaderID;
            log.otherinfo = result.status + " " + result.message;
            db.Logs.Add(log);
            db.SaveChanges();

            return Json(result);
        }
        public ActionResult DeleteHeader(int? id)
        {
            JsonArray res = new JsonArray();

            try
            {
                MealHeader detail = db.MealHeaders.Find(id);
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
            log.descriptions = "Delete Meal Header. Header ID:" + id;
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
                int cnt = 0;
                cnt = db.OrderDetails.Where(a => a.Status != "Deleted").Where(a => a.MealDetailsID == id).Count();
                if (cnt > 0)
                {
                    res.message = "There are orders that are linked with this item on Order Details. Consider editing item only.";
                    res.status = "failed";
                }
                else
                {
                    MealDetails detail = db.MealDetails.Find(id);
                    detail.Status = "Deleted";
                    db.Entry(detail).State = EntityState.Modified;
                    db.SaveChanges();
                    res.status = "success";
                }
               
            }
            catch (Exception ex)
            {
                res.message = ex.InnerException.InnerException.Message;
                res.status = "failed";
            }

            Log log = new Log();
            log.descriptions = "Delete Meal Details. Detail ID:" + id;
            log.otherinfo = res.status + " " + res.message;
            db.Logs.Add(log);
            db.SaveChanges();

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateMeal(MealViewModel item)
        {
            JsonArray result = new JsonArray();

            int ConcessionaireID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();

            int headerID = db.MealHeaders.Where(a=>a.Status == "Active").Where(a => a.Date == item.Date).Where(a => a.ConcessionaireID == ConcessionaireID).Select(a => a.ID).FirstOrDefault();

            if (headerID == 0)
            {
                MealHeader header = new MealHeader();
                header.ConcessionaireID = ConcessionaireID;
                header.Date = item.Date;
                db.MealHeaders.Add(header);
                db.SaveChanges();

                headerID = header.ID;

            }


            MealDetails detail = new MealDetails();
            detail.MealType = item.MealType;
            detail.Name = item.Name;
            detail.Descriptions = item.Descriptions;
            detail.NoServings = item.NoServings;
            detail.MealHeaderID = headerID;

            db.MealDetails.Add(detail);
            db.SaveChanges();

            Log log = new Log();
            log.descriptions = "Create Meal Header. Header ID:" + headerID;
            log.otherinfo = result.status + " " + result.message;
            db.Logs.Add(log);
            db.SaveChanges();

            Log logs = new Log();
            logs.descriptions = "Create Meal Detail. Detail ID:" + detail.ID;
            logs.otherinfo = result.status + " " + result.message;
            db.Logs.Add(logs);
            db.SaveChanges();


            result.num1 = headerID;
            result.status = "success";
            return Json(result);
        }
        public ActionResult getDataDetails(int draw, int start, int length, string strcode, int? headerID, int? noCols)
        {
            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;

            var v = db.MealDetails
                    .Where(b => b.MealHeaderID == headerID)
                    .Where(b => b.Status == "Active")
                    .Select(b =>
                            new
                            {

                                b.ID,
                                b.Name,
                                b.MealType,
                                b.Descriptions,
                                b.NoServings

                            }
                    );

            recordsTotal = v.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.Name.ToUpper().Contains(searchValue.ToUpper())
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
                string col = Request["columns[" + sortColumn + "][data]"];
                v = v.OrderBy(col + " " + sortColumnDir);
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
        public JsonResult GetDateDetails(DateTime Date)
        {
            JsonArray res = new JsonArray();
            int ConcessionaireID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();
            try
            {
                var itemList = db.MealHeaders
                .Where(a => a.Status == "Active")
                .Where(a => a.Date == Date)
                .Where(a => a.ConcessionaireID == ConcessionaireID)

               .Select(a => new
               {

                   MealHeaderID = a.ID

               }).ToList();







                return Json(itemList);
            }
            catch (Exception e)
            {
                res.message = e.Message;
                res.status = "fail";
                return Json(res);
            }


        }
        [HttpPost]
        public JsonResult CopyMeal(int id, DateTime Date)
        {
            int ConcessionaireID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();
            JsonArray res = new JsonArray();
            string i = "";

            try
            {

                int cnt = db.MealHeaders.Where(a => a.Status == "Active").Where(a => a.ConcessionaireID == ConcessionaireID).Where(a => a.Date == Date).Count();
                if (cnt > 0)
                {
                    res.message = "Date already existing";
                    res.status = "fail";
                }
                else
                {
                    MealHeader mh = new MealHeader();
                    mh.ConcessionaireID = ConcessionaireID;
                    mh.Date = Date;
                    db.MealHeaders.Add(mh);
                    db.SaveChanges();

                    int mhID = mh.ID;

                    var items = db.MealDetails.Where(a => a.Status == "Active").Where(a => a.MealHeaderID == id).ToList();
                    foreach (var item in items)
                    {
                        MealDetails md = new MealDetails();
                        md.MealType = item.MealType;
                        md.NoServings = item.NoServings;
                        md.MealHeaderID = mhID;
                        md.Name = item.Name;
                        md.Descriptions = item.Descriptions;
                        db.MealDetails.Add(md);
                        db.SaveChanges();

                    }
                    res.status = "success";

                }
                return Json(res);
            }
            catch (Exception e)
            {
                res.message = e.Message;
                res.status = "fail";
                return Json(res);
            }


        }
    }
}