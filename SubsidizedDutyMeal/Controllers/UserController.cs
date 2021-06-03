using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SubsidizedDutyMeal.DAL;
using SubsidizedDutyMeal.Models;
using SubsidizedDutyMeal.Models.View_Model;

namespace SubsidizedDutyMeal.Controllers
{
    public class UserController : Controller
    {
        private SDMContext db = new SDMContext();
        // GET: User
        public ActionResult Index(int? page, string searchString, string domain)
        {
            ViewBag.UserRole = new SelectList(db.Roles, "ID", "Name");
            ViewBag.Domain = domain;
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

            var v = db.Users
                    .Where(b => b.status == "Enabled")
                    .Select(b =>
                            new
                            {

                                b.id,
                                b.Username, 
                                b.Name,
                                b.Roles
                            }
                    );

            recordsTotal = v.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.Username.ToUpper().Contains(searchValue.ToUpper())
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
        public ActionResult Delete(int? id)
        {
            JsonArray res = new JsonArray();

            try
            {
                User detail = db.Users.Find(id);
                detail.status = "Deleted";
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
        public ActionResult Register()
        {
            ViewBag.UserRole = new SelectList(db.Roles, "ID", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel reg)
        {
            var role = db.Roles.Find(reg.UserRole);


                try
                {

                    User user = new User();

                    user.Username = reg.Username;
                    user.Roles = role.Name;
                    user.Password = GetSHA1HashData(reg.Password);
                    user.status = "Enabled";
                    db.Users.Add(user);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("", "Invalid registration.");
                }

            return View(reg);
        }
        private string GetSHA1HashData(string data)
        {
            //create new instance of md5
            SHA1 sha1 = SHA1.Create();

            //convert the input text to array of bytes
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(data));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            // return hexadecimal string
            return returnValue.ToString();
        }
        public JsonResult ShowDetails(int? id)
        {

            JsonArray result = new JsonArray();
            try
            {
                var item = db.Users.Find(id);

                result.num1 = db.Roles.Where(a=>a.Name == item.Roles).Select(a=>a.ID).FirstOrDefault();
                result.param1 = item.Roles;
                result.status = "success";
            }
            catch (Exception ex)
            {


                result.message = ex.Message;
                result.status = "fail";
            }





            return Json(result);
        }
        [HttpPost]
        public ActionResult Update(int ID,string UserRole)
        {

            JsonArray result = new JsonArray();

            try
            {
                var user = db.Users.Find(ID);
                user.Roles = UserRole;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

          
                result.status = "success";

            }
            catch (Exception ex)
            {
                result.status = "fail";
                result.message = ex.InnerException.Message.ToString();
            }

            return Json(result);
        }

    }
}