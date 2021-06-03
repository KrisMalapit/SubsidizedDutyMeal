using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models
{
    public class MealOverRide
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public string Details { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime Created_At { get; set; }
        public string Status { get; set; }
        public MealOverRide()
        {
            Status = "Active";
            Created_At = DateTime.Now.Date;
        }
    }
}