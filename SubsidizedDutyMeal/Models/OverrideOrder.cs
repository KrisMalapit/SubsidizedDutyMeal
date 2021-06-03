using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace SubsidizedDutyMeal.Models
{
    public class OverrideOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public int ConcessionaireID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime Created_Date { get; set; }
        public OverrideOrder()
        {
            Created_Date = DateTime.Now;

           


        }
    }
}