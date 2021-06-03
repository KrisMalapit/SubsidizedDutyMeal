using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models.View_Model
{
    public class MealScheduleViewModel
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string OrderType { get; set; }
        public string MealName { get; set; }
        public string MealType { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }
        public string Details { get; set; }

    }
}