using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models
{
    public class BiometricIP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        //[Index("IX_Concessionaire", 1, IsUnique = true)]
        public int ConcessionaireID { get; set; }
        [Index("IX_IPAddress", 1, IsUnique = true)]
        [StringLength(20)]
        public string BiometricIPAddress { get; set; }
        public DateTime LastRun { get; set; }
    }
}