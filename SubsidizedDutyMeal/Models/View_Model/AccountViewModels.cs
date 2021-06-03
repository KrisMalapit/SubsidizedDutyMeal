using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models.View_Model
{
    //public enum UserRoles
    //{
    //    Admin, User, Warehouse_Admin, Warehouse_Staff, SuperAdmin
    //}

    public class LoginViewModel
    {


        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
        //[Required]
        [Display(Name = "Domain")]
        public string Domain { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public int UserRole { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class AccountViewModels
    {
    }
}