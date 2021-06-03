    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubsidizedDutyMeal.Models
{
    public enum Domain
    {
        SMCDACON
    }
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]

        [Index(IsUnique = true)]
        public string Username { get; set; }
        public string Roles { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string mail { get; set; }
        public string status { get; set; }
        public string sysid { get; set; }
        public string Name { get; set; }

        //public virtual Role Roles { get; set; }
    }
}