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
    public class MealScheduleController : Controller
    {
        private SDMContext db = new SDMContext();
        // GET: MealSchedule
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

        

            var v = db.OrderHeaders
                    .Where(b => b.Status != "Deleted")
                    //.Where(b => b.Status == "Active")
                    .Where(b => b.Date == dt)
                    .Select(b =>
                            new MealScheduleViewModel()
                            {

                               ID = b.ID,
                               Date = b.Date,
                               EmployeeID = b.EmployeeID,
                               EmployeeName = db.Employees.Where(c => c.EmployeeID == b.EmployeeID).Select(c => c.LastName + ", " + c.FirstName).FirstOrDefault(),
                               Status  = b.Status
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
        public JsonResult ShowDetails(int?id) {

            JsonArray result = new JsonArray();



            var details = db.OrderDetails.Where(b=>b.OrderHeaderID == id).FirstOrDefault();

            result.param1 = db.Employees.Where(a => a.EmployeeID == details.OrderHeaders.EmployeeID).Select(b => b.LastName + ", " + b.FirstName).FirstOrDefault();
                            
            result.param2 = details.MealDetails.Name;
            result.param3 = details.MealDetails.MealType;
            
          
            result.status = "success";
            return Json(result);

        }
        public JsonResult VoidMeal(VoidMeal item)
        {

            JsonArray res = new JsonArray();

            
            try
            {
                int cnt = db.OrderDetails.Where(a => a.OrderHeaders.ID == item.OrderHeaderID).Where(a => a.Status == "Served").Count();
                if (cnt > 0)
                {
                    res.message = "Cannot delete. Some items related to this item has already been served.";
                    res.status = "failed";
                }
                else
                {
                    OrderHeader det = db.OrderHeaders.Find(item.OrderHeaderID);
                    if (det.Status != "Active")
                    {
                        res.message = "Item already been " + det.Status;
                        res.status = "failed";
                    }
                    else
                    {
                        det.Status = "Cancelled";
                        db.Entry(det).State = EntityState.Modified;
                        db.SaveChanges();
                        db.VoidMeals.Add(item);
                        db.SaveChanges();
                        res.status = "success";
                    }
                    

                    




                }
            }
            catch (Exception e)
            {
                res.message = e.Message;
                res.status = "failed";
               
            }



            Log log = new Log();
            log.descriptions = "Cancel Order Header. Header ID:" + item.OrderHeaderID;
            log.otherinfo = res.status + " " + res.message;
            db.Logs.Add(log);
            db.SaveChanges();



            return Json(res);

        }
    }
}