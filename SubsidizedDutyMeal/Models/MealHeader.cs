using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models
{
    public class MealHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime Date { get; set; }
        public DateTime Created_At { get; set; }
        public int ConcessionaireID { get; set; }
        public string Status { get; set; }


        public MealHeader(){
            Status = "Active";
            Created_At = DateTime.Now.Date;
        }
    }
}