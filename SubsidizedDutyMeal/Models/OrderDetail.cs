using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int OrderHeaderID { get; set; }
        public int MealDetailsID { get; set; }
        public DateTime Created_At { get; set; }
        public string Status { get; set; }
        public string OrderType { get; set; }
        public virtual OrderHeader OrderHeaders { get; set; }
        public virtual MealDetails MealDetails { get; set; }
        public DateTime Updated_At { get; set; } = DateTime.Now.Date;
        public OrderDetail()
        {
            Status = "Pending";
            Created_At = DateTime.Now.Date;
        }
    }
}