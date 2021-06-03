using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models.View_Model
{
    public class OrderViewModel
    {
        public DateTime Date { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int OrderHeaderID { get; set; }
        public int MealDetailsID { get; set; }
        public string MealName { get; set; }
        public string MealType { get; set; }
        public string OrderType { get; set; }
        public string AllowedMealType { get; set; }
        public int ID { get; set; }
        public string Status { get; set; }

    }
}