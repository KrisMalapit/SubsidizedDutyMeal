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
    public class MealOverRideController : Controller
    {
        private SDMContext db = new SDMContext();
        // GET: MealOverRide
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult getData(int draw, int start, int length, string strcode, string domain, int noCols, DateTime? dt)
        {
            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;



            var v = db.MealOverRide

                    .Where(b => b.Status == "Active")
                    //.Where(b => b.OrderDate == dt)
                    .Select(b =>
                            new MealScheduleViewModel()
                            {

                                ID = b.ID,
                                Date = b.OrderDate,
                                EmployeeID = b.EmployeeID,
                                EmployeeName = db.Employees.Where(a => a.EmployeeID == b.EmployeeID).Select(a => a.LastName + ", " + a.FirstName).FirstOrDefault(),
                                            
                                Details = b.Details,
                                Status = b.Status
                            }
                    );





            recordsTotal = v.Count();


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

        public JsonResult CreateOverRide(MealOverRide item)
        {

            JsonArray result = new JsonArray();
            try
            {
                db.MealOverRide.Add(item);
                db.SaveChanges();
           
                result.status = "success";
            }
            catch (Exception ex)
            {


                result.message = ex.Message;
                result.status = "fail";
            }


            Log logs = new Log();
            logs.descriptions = "Create Over Ride,ID:" + item.ID;
            logs.otherinfo = result.status + " " + result.message;
            db.Logs.Add(logs);
            db.SaveChanges();


            return Json(result);
        }
        public JsonResult Delete(int?id)
        {
            int userid = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();

            JsonArray result = new JsonArray();
            try
            {
                MealOverRide detail = db.MealOverRide.Find(id);
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
            log.descriptions = "Delete Meal OverRide. OverRide ID:" + id;
            log.otherinfo = result.status + " " + result.message;
            log.user_id = userid;
            db.Logs.Add(log);
            db.SaveChanges();


            return Json(result);
        }
    }
}