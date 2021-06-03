using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace SubsidizedDutyMeal.Models
{
    interface ICustomPrincipal : IPrincipal
    {
        int UserId { get; set; }
        string Role { get; set; }
    }

    public class CustomPrincipal : ICustomPrincipal
    {
        public int UserId { get; set; }
        public string Role { get; set; }

        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) { return false; }

        public CustomPrincipal(string email)
        {
            this.Identity = new GenericIdentity(email);
        }



    }
    public class CustomPrincipalSerializeModel
    {
        public int UserId { get; set; }
        public string Role { get; set; }
    }

}