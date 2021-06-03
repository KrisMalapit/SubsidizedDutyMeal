using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using SubsidizedDutyMeal.DAL;
using SubsidizedDutyMeal.Models;


namespace SubsidizedDutyMeal.Controllers
{
 
    public class ScheduleMatrixController : Controller
    {
        private SDMContext db = new SDMContext();
        // GET: ScheduleMatrix
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult getData(int draw, int start, int length, string strcode, string domain, int noCols)
        {
            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;



            var v = db.ScheduleMatrixes.Where(a=>a.Status=="Active")
                    .Select(b =>
                            new
                            {
                                b.ID,
                                b.Code,
                                b.Description,
                                b.MealType
                                
                            }
                    );





            recordsTotal = v.Count();


            DateTime dval = new DateTime();
            bool hasDateCreated = false;
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
            var lstData = v.OrderByDescending(a => a.ID).Skip(skip).Take(pageSize).ToList();



            var model = new
            {
                draw = draw,
                recordsFiltered = recordsFiltered,
                recordsTotal = recordsTotal,
                data = lstData
            };
            return Json(model, JsonRequestBehavior.AllowGet);



        }
        public JsonResult CreateUpdate(ScheduleMatrix item, string Ttype)
        {

            JsonArray result = new JsonArray();
            try
            {
                if (Ttype == "new")
                {
                    result.message = "NEW";
                    db.ScheduleMatrixes.Add(item);
                    db.SaveChanges();

                }
                else
                {
                    result.message = "UPDATE";
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();

                }


                result.status = "success";
            }
            catch (Exception ex)
            {



                result.message = "Duplicate entries found";
                result.status = "fail";
            }


            //Log logs = new Log();
            //logs.descriptions = "Biometric IP,ID:" + item.ID;
            //logs.otherinfo = result.status + " " + result.message;
            //db.Logs.Add(logs);
            //db.SaveChanges();


            return Json(result);
        }
        public JsonResult Delete(int? id)
        {
            int userid = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();

            JsonArray result = new JsonArray();
            try
            {
                ScheduleMatrix detail = db.ScheduleMatrixes.Find(id);

                detail.Status = "Deleted";
                db.Entry(detail).State = EntityState.Modified;
                db.SaveChanges();

                result.status = "success";
            }
            catch (Exception ex)
            {
                result.message = ex.InnerException.InnerException.Message;
                result.status = "failed";
            }

            Log log = new Log();
            log.descriptions = "Delete ScheduleMatrixes Record. ID:" + id;
            log.otherinfo = result.status + " " + result.message;
            log.user_id = userid;
            db.Logs.Add(log);
            db.SaveChanges();


            return Json(result);
        }
        public JsonResult ShowDetails(int? id)
        {

            JsonArray result = new JsonArray();
            try
            {
                var item = db.ScheduleMatrixes.Find(id);
                result.param1 = item.MealType;
                result.param2 = item.Code;
                result.param3 = item.Description;


                result.status = "success";
            }
            catch (Exception ex)
            {


                result.message = ex.Message;
                result.status = "fail";
            }





            return Json(result);
        }
    }
}