using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SubsidizedDutyMeal.DAL;
using SubsidizedDutyMeal.Models;

namespace SubsidizedDutyMeal.Controllers
{
    public class EmployeeController : Controller
    {
        private SDMContext db = new SDMContext();
        // GET: Employee
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult getData(int draw, int start, int length, string strcode, int noCols)
        {

            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;

            var v = db.Employees
                    .Where(b => b.Status == "Active")
                    .Select(b =>
                            new
                            {
                                b.ID,
                                b.EmployeeID,
                                b.LastName,
                                b.FirstName,
                               
                                b.Position,
                                b.Status,
                               
                            }
                    );

            recordsTotal = v.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.LastName.ToUpper().Contains(searchValue.ToUpper()) ||
                    a.FirstName.ToString().Contains(searchValue.ToUpper()) ||
                    a.Position.ToString().Contains(searchValue.ToUpper())
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
                            //strFilter = (colSearch != "EmployeeID") ? colSearch + ".Contains(" + "\"" + colval + "\"" + ")" : "(" + colSearch + "=" + colval + ")";
                        }
                    }
                    else
                    {

                        strFilter = strFilter + " && " + colSearch + ".Contains(" + "\"" + colval + "\"" + ")";
                        //strFilter = (colSearch != "EmployeeID") ? strFilter + " && " + colSearch + ".Contains(" + "\"" + colval + "\"" + ")" : strFilter + " && " + "(" + colSearch + "=" + colval + ")";
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
        public ActionResult Delete(int? id)
        {
            JsonArray res = new JsonArray();

            try
            {
                Employee detail = db.Employees.Find(id);
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

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Create()
        {
            //ViewBag.EmploymentStatus = new SelectList(db.EmploymentStatus.Where(b => b.Status == "Active"), "ID", "Name");
            //ViewBag.DivisionID = new SelectList(db.Divisions.Where(b => b.Status == "Active"), "ID", "Name");
            //ViewBag.DepartmentID = new SelectList(db.Departments.Where(b => b.Status == "Active"), "ID", "Name");
            //ViewBag.CompanyID = new SelectList(db.Companies.Where(b => b.Status == "Active"), "ID", "Name");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee item)
        {
            //try
            //{
            //    if (ModelState.IsValid)
            //    {
            //        db.Employees.Add(item);
            //        db.SaveChanges();
            //        return RedirectToAction("Index");
            //    }
            //}
            //catch (Exception ex)
            //{

            //    ModelState.AddModelError("EmployeeID", "Unable to save changes. Duplicate Entry.");
            //}



            try
            {
                if (ModelState.IsValid)
                {
                    db.Employees.Add(item);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {

                ModelState.AddModelError("EmployeeID", e.InnerException.InnerException.Message);
                return View(item);
            }



            return View(item);
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Employee item = db.Employees.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            ViewBag.EmploymentStatus = new SelectList(db.EmploymentStatus.Where(b => b.Status == "Active"), "ID", "Name",item.EmploymentStatus);
            ViewBag.DivisionID = new SelectList(db.Divisions.Where(b => b.Status == "Active"), "ID", "Name",item.DivisionID);
            ViewBag.DepartmentID = new SelectList(db.Departments.Where(b => b.Status == "Active"), "ID", "Name",item.DepartmentID);
            ViewBag.CompanyID = new SelectList(db.Companies.Where(b => b.Status == "Active"), "ID", "Name",item.CompanyID);
            ViewBag.ConcessionaireID = new SelectList(db.Concessionaires.Where(b => b.Status == "Active"), "ID", "Name", item.ConcessionaireID);
            string dt = String.Format("{0:MM/dd/yyyy}", item.ContractEndDate);         
            ViewBag.ContractDate = dt;
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee item)
        {
            if (ModelState.IsValid)
            {
                item.Status = "Active";
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(item);
        }
        public JsonResult SelectEmploymentStatus(string q)
        {

            var model = db.EmploymentStatus.Where(b => b.Status == "Active").Select(b => new
            {

                id = b.ID,
                text =  b.Name,
            });

            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(b => b.text.Contains(q));
            }

            var modelDept = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelDept, JsonRequestBehavior.AllowGet);
        }
    }
}