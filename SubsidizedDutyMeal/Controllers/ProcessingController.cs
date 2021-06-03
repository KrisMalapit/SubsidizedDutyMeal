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
    public class ProcessingController : Controller
    {
        private SDMContext db = new SDMContext();
        // GET: Processing
        public ActionResult Index()
        {
           
            return View();
        }
        public JsonResult GetMeal(string EmployeeID, string MealType)
        {

            int ConcessionaireID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();

            DateTime dt = DateTime.Now.Date;
            //DateTime dt = new DateTime(2019, 2, 2);
            string EmpName = "";
            EmpName = db.Employees.Where(a => a.EmployeeID == EmployeeID).Select(a => a.LastName + ", " + a.FirstName).FirstOrDefault();
                      
            if (string.IsNullOrEmpty(EmpName))
            {
                EmpName = "";
            }

            var itemList = db.OrderDetails
                .Where(a=>a.OrderHeaders.Status == "Active")
                .Where(a => a.Status != "Deleted")

                .Where(a => a.OrderHeaders.Date == dt)
                .Where(a => a.OrderHeaders.EmployeeID == EmployeeID)
                .Where(a => a.MealDetails.MealType == MealType)
                .Where(a => a.MealDetails.MealHeaders.ConcessionaireID == ConcessionaireID)
               .Select(a => new OrderViewModel()
               {
                   ID = a.ID
                   ,
                   OrderHeaderID = a.OrderHeaders.ID
                   ,
                   MealName = a.MealDetails.Name
                   ,
                   MealType = a.MealDetails.MealType
                   ,
                   Status = a.Status
                   ,
                   Date = a.OrderHeaders.Date
                   ,
                   EmployeeID = EmployeeID
                   ,
                   EmployeeName = EmpName
               }).ToList();




            return Json(itemList);


        }
        public JsonResult ServedMeal(int orderID,string itemstat)
        {
            JsonArray res = new JsonArray();
            int ConcessionaireID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();
            try
            {
                OrderDetail detail = db.OrderDetails.Find(orderID);
                if (detail.Status == "Served" || detail.Status == "UnServed")
                {
                    res.message = "Item already " + detail.Status;
                    res.status = "fail";
                }
                else
                {
                    if (itemstat == "UnServed")
                    {
                        detail.Status = "UnServed";
                    }
                    if (itemstat == "Served")
                    {
                        detail.Status = "Served";
                    }
                   
                    db.Entry(detail).State = EntityState.Modified;
                    db.SaveChanges();


                    if (itemstat == "UnServed")
                    {
                        OverrideOrder ov = new OverrideOrder();
                        ov.ConcessionaireID = ConcessionaireID;
                        ov.EmployeeID = detail.OrderHeaders.EmployeeID;
                        ov.OrderDate = detail.OrderHeaders.Date;
                        db.OverrideOrders.Add(ov);
                        db.SaveChanges();
                    }



                    res.status = "success";
                }
               
                
            }
            catch (Exception ex)
            {
                res.message = ex.InnerException.InnerException.Message;
                res.status = "failed";
            }

            Log log = new Log();
            log.descriptions = "Served Meal. Order Detail ID: " + orderID + " with itemstat :" + itemstat;
            log.otherinfo = res.status + " " + res.message;
            db.Logs.Add(log);
            db.SaveChanges();

            return Json(res);
        }
        public ActionResult getData(int draw, int start, int length, string strcode, string domain, int noCols, DateTime ? Date)
        {
            DateTime dt = DateTime.Now.Date;
            //DateTime dt = new DateTime(2019,2,13);

            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;

            var v = db.OrderDetails
                    .Where(b=>b.OrderHeaders.Date == dt)
                    .Where(b => b.OrderHeaders.Status == "Active")
                    .Where(b => b.Status != "Deleted")

                    .Select(b =>
                            new MealScheduleViewModel()
                            {

                              ID = b.ID,
                              Date =  b.OrderHeaders.Date,
                              EmployeeID = b.OrderHeaders.EmployeeID,
                              EmployeeName = db.Employees.Where(a => a.EmployeeID == b.OrderHeaders.EmployeeID).Where(a => (a.Position.ToUpper() ?? "").Contains("OPERATOR")).Select(a => a.FirstName + " " + a.LastName).FirstOrDefault(),
                              Position = db.Employees.Where(a => a.EmployeeID == b.OrderHeaders.EmployeeID).Select(a => a.Position).FirstOrDefault(),
                              MealType = b.MealDetails.MealType,
                              MealName = b.MealDetails.Name
                              ,Status = b.Status
                             }
                    );


            v = v.Where(a=>a.Position == "Programmer");

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
        public JsonResult LoadDataCarousel(string mealType) {
           
            DateTime dt = DateTime.Now.Date;

            int ConcessionaireID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();

            JsonArray result = new JsonArray();
            int cntNoOrders = db.OrderDetails.Where(a => a.OrderHeaders.Date == dt).Where(a => a.OrderHeaders.Status == "Active").Where(a => a.Status != "Deleted")
                .Where(a => a.MealDetails.MealType == mealType).Where(a => a.MealDetails.MealHeaders.ConcessionaireID == ConcessionaireID).Count();

            int cntNoDineIn = db.OrderDetails.Where(a => a.OrderHeaders.Date == dt).Where(a => a.OrderHeaders.Status == "Active").Where(a => a.Status != "Deleted").Where(a => a.OrderType == "Dine-In")
                .Where(a => a.MealDetails.MealType == mealType).Where(a => a.MealDetails.MealHeaders.ConcessionaireID == ConcessionaireID).Count();

            int cntNotakeOut = db.OrderDetails.Where(a => a.OrderHeaders.Date == dt).Where(a => a.OrderHeaders.Status == "Active").Where(a => a.Status != "Deleted").Where(a => a.OrderType == "Take-Out")
                .Where(a => a.MealDetails.MealType == mealType).Where(a => a.MealDetails.MealHeaders.ConcessionaireID == ConcessionaireID).Count();

            int cntNoServed = db.OrderDetails.Where(a => a.OrderHeaders.Date == dt).Where(a => a.OrderHeaders.Status == "Active").Where(a => a.Status == "Served")
                .Where(a => a.MealDetails.MealType == mealType).Where(a => a.MealDetails.MealHeaders.ConcessionaireID == ConcessionaireID).Count();

            int cntNoUnServed = db.OrderDetails.Where(a => a.OrderHeaders.Date == dt).Where(a => a.OrderHeaders.Status == "Active").Where(a => a.Status == "UnServed")
                .Where(a => a.MealDetails.MealType == mealType).Where(a => a.MealDetails.MealHeaders.ConcessionaireID == ConcessionaireID).Count();


            result.num1 = cntNoOrders;
            result.num2 = cntNoDineIn;
            result.num3 = cntNotakeOut;
            result.num4 = cntNoServed;
            result.num5 = cntNoUnServed;

            return Json(result);
        }

    }
}