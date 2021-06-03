using SubsidizedDutyMeal.DAL;
using SubsidizedDutyMeal.Models;
using SubsidizedDutyMeal.Models.View_Model;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SubsidizedDutyMeal.Controllers
{
   
    public class AccountController : Controller
    {
        private SDMContext db = new SDMContext();
        public User GetUserDetails(User user)
        {
            var users = db.Users.ToList();
            return users.Where(u => u.Username.ToLower() == user.Username.ToLower() &&
            u.Password == GetSHA1HashData(user.Password)).FirstOrDefault();
        }


        //Get
        //Account/Login
        [AllowAnonymous]

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                return View();
            }

                
        }


        private string GetServer()
        {
            DirectoryEntry de =
               new DirectoryEntry("LDAP://RootDSE");

            return de.Properties["defaultNamingContext"][0].ToString();
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

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
            //return RedirectToAction("LogIn", "Account");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = new User() { Username = model.Username, Password = model.Password };

            user = GetUserDetails(user);

            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(model.Username, false);
                var authTicket = new FormsAuthenticationTicket(1, user.Username, DateTime.Now, DateTime.Now.AddMinutes(2880), true, user.Roles);
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                HttpContext.Response.Cookies.Add(authCookie);

                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return this.Redirect(returnUrl);
                }

            }

            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

        }
    }
}