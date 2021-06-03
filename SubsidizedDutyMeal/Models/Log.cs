using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models
{
    public class Log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;

        public int id { get; set; }
        public string descriptions { get; set; }
        public string otherinfo { get; set; }
        public DateTime created_date { get; set; }
        public int user_id { get; set; }
        public string browser { get; set; }
        public Log()
        {
            created_date = DateTime.Now;

            browser = bc.Browser + " , Version : " + bc.Version;


        }
    }
}