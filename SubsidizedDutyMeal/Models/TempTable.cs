using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models
{
    public class TempTable
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}