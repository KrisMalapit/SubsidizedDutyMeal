using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models
{
    public class VoidMeal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int OrderHeaderID { get; set; }
        public string Reason { get; set; }
        public DateTime Created_At { get; set; }
        public VoidMeal()
        {
            Created_At = DateTime.Now.Date;
        }
    }
}