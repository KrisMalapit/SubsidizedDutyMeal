using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models
{

    public class MealDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string MealType { get; set; }
        public string Name { get; set; }
        public string Descriptions { get; set; }
        public int NoServings { get; set; }
        public int MealHeaderID { get; set; }
        public virtual MealHeader MealHeaders { get; set; }
        public string Status { get; set; }
        public MealDetails()
        {
            Status = "Active";
        }

    }
}