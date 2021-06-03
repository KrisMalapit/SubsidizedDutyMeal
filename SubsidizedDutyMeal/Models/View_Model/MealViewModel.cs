using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models.View_Model
{
    public class MealViewModel
    {
        public DateTime Date { get; set; }
        public int ConcessionaireID { get; set; }
        public string Name { get; set; }
        public string MealType { get; set; }
        
        public string Descriptions { get; set; }
        public int NoServings { get; set; }
    }
}