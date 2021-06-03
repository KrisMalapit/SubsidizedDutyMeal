using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models
{
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public string Taste { get; set; }
        public string TasteRemarks { get; set; }
        public string NutritionalValue { get; set; }
        public string NutritionalValueRemarks { get; set; }
        public string PresentationAppearance { get; set; }
        public string PresentationAppearanceRemarks { get; set; }
        public string Attentiveness { get; set; }
        public string AttentivenessRemarks { get; set; }
        public string Courtesy { get; set; }
        public string CourtesyRemarks { get; set; }
        public string Speed { get; set; }
        public string SpeedRemarks { get; set; }
        public string Appearance { get; set; }
        public string AppearanceRemarks { get; set; }
        public string OverallService { get; set; }
        public string OverallServiceRemarks { get; set; }
        public string Utensils { get; set; }
        public string UtensilsRemarks { get; set; }
        public string TablesChairs { get; set; }
        public string TablesChairsRemarks { get; set; }
        public string Floor { get; set; }
        public string FloorRemarks { get; set; }
        public string Ambiance { get; set; }
        public string AmbianceRemarks { get; set; }
        public string Comments { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now.Date;

  
        public string Status { get; set; } = "Active";

    }
}