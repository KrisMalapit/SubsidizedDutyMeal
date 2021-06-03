using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using SubsidizedDutyMeal.DAL;
using SubsidizedDutyMeal.Models;
using SubsidizedDutyMeal.Models.View_Model;

namespace SubsidizedDutyMeal.Controllers
{
    public class OrderController : Controller
    {
        private SDMContext db = new SDMContext();
        public ActionResult getData(int draw, int start, int length, string strcode, string domain, int noCols,string empid)
        {
            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;

            string eid = empid;

            var v = db.OrderHeaders
                    .Where(b => b.Status == "Active")
                    .Where(b => b.EmployeeID == eid)
                    .Select(b =>
                            new
                            {

                                b.ID,
                                b.Date,
                                b.EmployeeID

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
                draw,
                recordsFiltered,
                recordsTotal,
                data = lstData
            };

            return Json(model, JsonRequestBehavior.AllowGet);



        }
        public ActionResult getDataFeedback(int draw, int start, int length, string strcode, string domain, int noCols, string empid)
        {
            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;



            var v = db.Feedbacks
                    .Where(b => b.Status == "Active")
                    .Where(b=>b.EmployeeID == empid)
                    .Select(b =>
                            new
                            {

                                b.ID,
                                b.DateCreated,
                                Name = b.EmployeeID
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


                    if (colSearch == "DateCreated")
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
                v = v.Where("DateCreated = @0", dval);
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
        public JsonResult CreateUpdateFeedback(Feedback item, string Ttype, string EmployeeID)
        {

            JsonArray result = new JsonArray();

            try
            {
                if (Ttype == "new")
                {
                    result.message = "NEW";
                    db.Feedbacks.Add(item);
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
                result.message = ex.InnerException.InnerException.Message;
                result.status = "fail";
            }





            return Json(result);
        }
        public JsonResult DeleteFeedback(int? id)
        {
            int userid = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();

            JsonArray result = new JsonArray();
            try
            {
                Feedback detail = db.Feedbacks.Find(id);

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
            log.descriptions = "Delete Feedback Record. ID:" + id;
            log.otherinfo = result.status + " " + result.message;
            log.user_id = userid;
            db.Logs.Add(log);
            db.SaveChanges();


            return Json(result);
        }

        public JsonResult ShowDetailsFeedback(int? id)
        {
            var item = db.Feedbacks.Where(a=>a.ID == id).ToList();

            JsonArray result = new JsonArray();
            try
            {
                
                //result.num1 = item.ID;
                //result.param1 = item.Code;
                //result.param2 = item.Name;



                result.status = "success";
            }
            catch (Exception ex)
            {


                result.message = ex.Message;
                result.status = "fail";
            }

            var model = new
            {
                status = result.status,
                message = result.message,
                data = item

            };



            return Json(model);
        }
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Feedback()
        {
            return View();
        }
        public ActionResult Details(int? id, string EmpID, string EmpName,int?Concessionaire)
        {

            ViewBag.Header = id;
            ViewBag.EmpID = EmpID;

            int ConcessionaireID = Convert.ToInt32(Concessionaire);

            if (string.IsNullOrEmpty(EmpName))
            {
                EmpName = db.Employees.Where(a => a.EmployeeID == EmpID).Select(a => a.LastName + ", " + a.FirstName).FirstOrDefault();
                          
            }
            if (ConcessionaireID == 0)
            {
                Concessionaire = db.Employees.Where(a => a.EmployeeID == EmpID).Select(a => a.ConcessionaireID).FirstOrDefault(); 
                                 
            }


            var itemList = db.OrderDetails
                .Where(a => a.Status != "Deleted")
               .Where(a => a.OrderHeaderID == id)
 
               .Select(a => new OrderViewModel()
               {
                   ID = a.ID
                   ,MealName = a.MealDetails.Name
                   ,MealType = a.MealDetails.MealType
                   ,OrderType = a.OrderType
                   ,Status = a.Status
                   ,Date = a.OrderHeaders.Date
               }).ToList();
            if (itemList.Count != 0)
            {
                ViewBag.Date = itemList[0].Date.ToShortDateString();
            }
            

            ViewBag.EmpName = EmpName;
     


            ViewBag.ConcessionaireID = new SelectList(db.Concessionaires.Where(b => b.Status == "Active"), "ID", "Name", Concessionaire);
      




            return View(itemList);
        }


        public JsonResult GetEmployeeDetails(string EmpID)
        {
            JsonArray result = new JsonArray();
            string EmpName = "";
            string Concessionaire = "";
            string MealType = "";

            try
            {
                EmpName = db.Employees.Where(a => a.EmployeeID == EmpID).Select(a => a.LastName + ", " + a.FirstName).FirstOrDefault();
                          

                if (string.IsNullOrEmpty(EmpName))
                {
                   
                    result.message = "Employee ID does not exist!";
                    result.status = "fail";
                }
                else
                {
                    bool isBlocked = db.Employees.Where(a => a.EmployeeID == EmpID).Where(a=>a.Blocked == 1).Count() == 1 ? true : false;

                    if (isBlocked)
                    {
    
                        result.message = "You have been blocked! Contact your HR Administrator";
                        result.status = "fail";
                    }
                    else
                    {
                        Concessionaire = db.Employees.Where(a => a.EmployeeID == EmpID).Select(a => a.Concessionaires.Name).FirstOrDefault();
                        result.param1 = Concessionaire;
                        result.message = EmpName;
                        result.status = "success";
                    }

                   
                }
            }
            catch (Exception ex)
            {
                result.message = ex.InnerException.InnerException.Message;
                result.status = "fail";
            }



            return Json(result);
        }
        


        public ActionResult getDataMenu(int draw, int start, int length, string strcode, int noCols, int ConcessionaireID, string MealType, DateTime?Date)
        {

            //switch (MealType)
            //{

            //    case "1":
            //        MealType = "Lunch";
            //        break;
            //    case "2":
            //        MealType = "Dinner";
            //        break;
            //}

            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;

            int MHId = db.MealHeaders
                    .Where(b => b.Date == Date)
                    .Where(b => b.ConcessionaireID == ConcessionaireID)
                    .Where(b => b.Status == "Active")
                    .Select(b => b.ID)
                    .FirstOrDefault();

            var v = db.MealDetails
                    .Where(b => b.MealHeaderID == MHId)
                    .Where(b => b.MealType == MealType)
                    .Where(b => b.Status == "Active")
                    .Select(b =>
                            new
                            {
                                b.ID,
                                b.Name,
                                b.Descriptions,
                                b.NoServings,
                                ServingsLeft = 0
                            }
                    );

            recordsTotal = v.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.Name.ToUpper().Contains(searchValue.ToUpper()) ||
                    a.Descriptions.ToString().Contains(searchValue.ToUpper())
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


            //var lstData = v.Skip(skip).Take(pageSize).ToList();

            var lstData = v.Skip(skip).Take(pageSize).ToList().Select(a => new
            {
                a.ID,
                a.Name,
                a.Descriptions,

                ServingsLeft = a.NoServings - TotalInvOut(a.ID),

            });


            var model = new
            {
                draw = draw,
                recordsFiltered = recordsFiltered,
                recordsTotal = recordsTotal,
                data = lstData
            };
            return Json(model, JsonRequestBehavior.AllowGet);



        }
        public int TotalInvOut(int id)
        {



            int total_inv = db.OrderDetails
                     .Where(a => a.MealDetailsID == id)
                     .Where(a => a.Status != "Deleted")
                     .Where(a => a.Status != "UnServed")
                    .Count();




            return total_inv;


        }
        public JsonResult AddMealDetail(OrderViewModel item)
        {
           
            JsonArray result = new JsonArray();

            int cnt = db.OrderDetails.Where(a=>a.Status != "Deleted")
                .Where(a=>a.OrderHeaders.Status != "Deleted")
                .Where(a=>a.OrderHeaders.EmployeeID == item.EmployeeID)
                .Where(a => a.OrderHeaders.Date == item.Date)
                .Count();



            int Year = item.Date.Year;
            int Month = item.Date.Month;
            int Day = item.Date.Day;
            string sql = "D"+ Day + ".Contains(" + "\"OFF\"" + ")";

            int cntdayOff = db.ScheduleDetails
                .Where(s => s.ScheduleHeaders.Status == "Active")
                .Where(s => s.Status == "Active")
                .Where(s => s.EmployeeID == item.EmployeeID)
                .Where(s => s.Year == Year)
                .Where(s => s.Month == Month)
                .Where(sql).Count();


            if (cntdayOff > 0)
            {
                result.status = "fail";
                result.message = "Day OFF";
                return Json(result);
            }


            int cnthasSched = db.ScheduleDetails
                .Where(s => s.ScheduleHeaders.Status == "Active")
                .Where(s => s.Status == "Active")
                .Where(s => s.EmployeeID == item.EmployeeID)
                .Where(s => s.Year == Year)
                .Where(s => s.Month == Month).Count();


            if (cnthasSched == 0)
            {
                result.status = "fail";
                result.message = "No schedule set";
                return Json(result);
            }



            if (cnt > 0)
            {
                result.status = "fail";
                result.message = "Meal existing";
                return Json(result);
            }


            int noServings = db.MealDetails
                .Where(a => a.Status == "Active")
                .Where(a => a.ID == item.MealDetailsID)
                .Select(a => a.NoServings)
                .FirstOrDefault();

            int curInv = noServings - TotalInvOut(item.MealDetailsID);


            if (curInv < 1 )
            {
                result.status = "fail";
                result.message = "No more servings available";
                return Json(result);
            }


            int headerID = db.OrderHeaders.Where(a => a.Status == "Active")
                .Where(a => a.Date == item.Date)
                .Where(a => a.EmployeeID == item.EmployeeID)
                .Select(a => a.ID).FirstOrDefault();

            if (headerID == 0)
            {
                OrderHeader header = new OrderHeader();
                header.EmployeeID = item.EmployeeID;
                header.Date = item.Date;
                db.OrderHeaders.Add(header);
                db.SaveChanges();

                headerID = header.ID;

            }


            

            OrderDetail detail = new OrderDetail();
            detail.OrderHeaderID = headerID;
            detail.MealDetailsID = item.MealDetailsID;
            detail.OrderType = item.OrderType;
           
            db.OrderDetails.Add(detail);
            db.SaveChanges();

            var mealname = db.MealDetails
                .Where(a => a.Status == "Active")
                .Where(a => a.ID == item.MealDetailsID).Select(a => a.Name).FirstOrDefault();

            result.num1 = headerID;
            result.status = "success";
            result.num2 = detail.ID;
            result.param1 = mealname;
            result.param2 = item.MealType;
            result.param3 = "Pending";
            result.param4 = item.OrderType;


            Log log = new Log();
            log.descriptions = "Create Order Header. Header ID:" + headerID;
            log.otherinfo = result.status + " " + result.message;
            db.Logs.Add(log);
            db.SaveChanges();

            Log logs = new Log();
            logs.descriptions = "Create Order Detail. Detail ID:" + detail.ID;
            logs.otherinfo = result.status + " " + result.message;
            db.Logs.Add(logs);
            db.SaveChanges();


            return Json(result);
        }
        [HttpPost]
        public ActionResult DeleteHeader(int? id)
        {
            JsonArray res = new JsonArray();

            int cnt = db.OrderDetails.Where(a => a.OrderHeaders.ID == id).Where(a=>a.Status == "Served").Count();

            if (cnt > 0)
            {
                res.message = "Cannot delete. Some items related to this item has already been served.";
                res.status = "failed";
            }
            else
            {
                try
                {
                    OrderHeader detail = db.OrderHeaders.Find(id);
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
                log.descriptions = "Delete Order Header. Header ID:" + id;
                log.otherinfo = res.status + " " + res.message;
                db.Logs.Add(log);
                db.SaveChanges();
            }

            

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteDetails(int? id)
        {
            JsonArray res = new JsonArray();

            try
            {
                OrderDetail detail = db.OrderDetails.Find(id);
                DateTime orderdate = detail.OrderHeaders.Date;

                TimeSpan diffResult = orderdate.Subtract(DateTime.Now);

                if ((diffResult.Days + 1) > 1)
                {

                        detail.Status = "Deleted";
                        db.Entry(detail).State = EntityState.Modified;
                        db.SaveChanges();
                        res.status = "success";
                
                } 
                else {
                    if (detail.Status == "UnServed")
                    {
                        detail.Status = "Deleted";
                        db.Entry(detail).State = EntityState.Modified;
                        db.SaveChanges();
                        res.status = "success";

                    }
                    else
                    {
                        res.message = "Not allowed to delete";
                        res.status = "fail";
                    }

                }

                


            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.status = "failed";
            }

            Log log = new Log();
            log.descriptions = "Delete Delete Details. Detail ID:" + id;
            log.otherinfo = res.status + " " + res.message;
            db.Logs.Add(log);
            db.SaveChanges();

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateMeal(int? id,int mealid)
        {
            JsonArray res = new JsonArray();

            try
            {
                OrderDetail detail = db.OrderDetails.Find(id);
                DateTime orderdate = detail.OrderHeaders.Date;

                string EmployeeID = detail.OrderHeaders.EmployeeID;

                TimeSpan diffResult = orderdate.Subtract(DateTime.Now);

                if (detail.MealDetailsID == mealid)
                {
                    res.message = "Cannot change same meal";
                    res.status = "fail";
                }
                else
                {
                    if ((diffResult.Days + 1) > 1)
                    {
                        detail.MealDetailsID = mealid;
                        detail.Status = "Pending";
                        db.Entry(detail).State = EntityState.Modified;
                        db.SaveChanges();
                        res.status = "success";

                    }
                    else
                    {
                        if (detail.Status == "UnServed")
                        {
                            detail.MealDetailsID = mealid;
                            detail.Status = "Pending";
                            db.Entry(detail).State = EntityState.Modified;
                            db.SaveChanges();
                            res.status = "success";

                        }
                        else
                        {
                            int cnt = db.MealOverRide.Where(a => a.EmployeeID == EmployeeID).Where(a => a.OrderDate == orderdate).Where(a => a.Status == "Active").Count();
                            if (cnt > 0)
                            {
                                detail.MealDetailsID = mealid;
                                detail.Status = "Pending";
                                db.Entry(detail).State = EntityState.Modified;
                                db.SaveChanges();
                                res.status = "success";
                            }
                            else
                            {
                                res.message = "Not allowed to change meal";
                                res.status = "fail";
                            }


                            
                        }

                    }
                }

                

                




            }
            catch (Exception ex)
            {
                res.message = ex.InnerException.InnerException.Message;
                res.status = "failed";
            }

            Log log = new Log();
            log.descriptions = "Update OrderDetails Detail ID:" + id + " . Change meal.";
            log.otherinfo = res.status + " " + res.message;
            db.Logs.Add(log);
            db.SaveChanges();

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        


        [HttpPost]
        public JsonResult GetDateDetails(DateTime Date,string EmployeeID) {

                var itemList = db.OrderDetails
                .Where(a => a.OrderHeaders.Status == "Active")
                .Where(a => a.Status != "Deleted")
                .Where(a => a.OrderHeaders.Date == Date)
                .Where(a => a.OrderHeaders.EmployeeID == EmployeeID)

               .Select(a => new OrderViewModel()
               {
                   ID = a.ID
                   , OrderHeaderID = a.OrderHeaders.ID
                   , MealName = a.MealDetails.Name
                   , MealType = a.MealDetails.MealType
                   , OrderType = a.OrderType
                   , Status = a.Status
                   , Date = a.OrderHeaders.Date
                   , EmployeeID = EmployeeID
               }).ToList();


            try
            {
                int Year = Date.Date.Year;
                int Month = Date.Date.Month;
                int Day = Date.Date.Day;
                string dynamicCol = "D" + Day;
                string schedCode = sFunc(dynamicCol, EmployeeID, Year, Month);
                string mealType = db.ScheduleMatrixes.Where(a => a.Code == schedCode).Select(a => a.MealType).FirstOrDefault();
                ViewBag.MealType = (string.IsNullOrEmpty(mealType)) ? "N/A" : mealType;
                if (itemList.Count != 0)
                {
                    ViewBag.Date = itemList[0].Date.ToShortDateString();
                    ViewBag.OrderHeaderID = itemList[0].OrderHeaderID;
                    
                }
                
            }
            catch (Exception e)
            {
                
                throw;
            }

            return Json(itemList, JsonRequestBehavior.AllowGet);


        }
        [HttpPost]
        public JsonResult GetMealType(DateTime Date, string EmployeeID)
        {
            JsonArray res = new JsonArray();
            try
            {
                int Year = Date.Date.Year;
                int Month = Date.Date.Month;
                int Day = Date.Date.Day;
                string dynamicCol = "D" + Day;
                string schedCode = sFunc(dynamicCol, EmployeeID, Year, Month);
                string mealType = db.ScheduleMatrixes.Where(a => a.Code == schedCode).Select(a => a.MealType).FirstOrDefault();
                res.message = (string.IsNullOrEmpty(mealType)) ? "N/A" : mealType;
                res.status = "success";

            }
            catch (Exception e)
            {
                res.message = e.Message;
                res.status = "fail";
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public string sFunc(string sCol,string empID,int Year,int Month)
        {
            var _tableRepository = db.ScheduleDetails.Where(x => x.ScheduleHeaders.Status == "Active")
                .Where(s => s.Status == "Active")
                .Where(s => s.EmployeeID == empID)
                .Where(s => s.Year == Year)
                .Where(s => s.Month == Month)
                .Select(e => e).FirstOrDefault();

            if (_tableRepository == null) return "";

            var _value = _tableRepository.GetType().GetProperties().Where(a => a.Name == sCol).Select(p => p.GetValue(_tableRepository, null)).FirstOrDefault();

            return _value.ToString();
        }



        public JsonResult GetOverRide(string EmployeeID,DateTime dt) {
            JsonArray result = new JsonArray();
            int cnt = db.OverrideOrders.Where(a => a.EmployeeID == EmployeeID).Where(a=>a.OrderDate == dt).Count();
            if (cnt > 0)
            {
                result.status = "success";
            }
            else
            {

                cnt = db.MealOverRide.Where(a => a.EmployeeID == EmployeeID).Where(a => a.OrderDate == dt).Where(a=>a.Status == "Active").Count();
                if (cnt > 0)
                {
                    result.status = "success";
                }
                else
                {
                    result.message = "You are only allowed to add item/s 2 days before Order Date";
                    result.status = "fail";
                }
                
            }


            return Json(result);

        }
        public JsonResult GetListMeals(int ConcessionaireID, string MealType, DateTime Date)
        {


            int MHId = db.MealHeaders
                    .Where(b => b.Date == Date)
                    .Where(b => b.ConcessionaireID == ConcessionaireID)
                    .Where(b => b.Status == "Active")
                    .Select(b => b.ID)
                    .FirstOrDefault();

            var v = db.MealDetails
                    .Where(b => b.MealHeaderID == MHId)
                    .Where(b => b.MealType == MealType)
                    .Where(b => b.Status == "Active")
                    .Select(b =>
                            new
                            {

                              id =  b.ID,
                              text =  b.Name
                            }
                    ).ToList();




          


            return Json(v);

        }
        public JsonResult ShowDetails(int?id)
        {
            JsonArray result = new JsonArray();
            var item = db.OrderDetails.Find(id);
            result.num1 = item.MealDetails.ID;
            result.param1 = item.MealDetails.Name;
            result.param2 = item.MealDetails.MealType;



            return Json(result);

        }
        public JsonResult GetInventory(int id)
        {
            JsonArray result = new JsonArray();

            var item = db.MealDetails.Find(id);
            result.num1 = item.NoServings - TotalInvOut(id);



            return Json(result);

        }


    }
}