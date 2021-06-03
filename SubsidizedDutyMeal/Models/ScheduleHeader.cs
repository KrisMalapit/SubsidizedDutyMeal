using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models
{
    public class ScheduleHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string ReferenceNo { get; set; }

        [Index("MonthYear", 1, IsUnique = true)]

        public int Month { get; set; }
        [Index("MonthYear", 2, IsUnique = true)]

        public int Year { get; set; }

        public string Status { get; set; }

        [Index("MonthYear", 3, IsUnique = true)]
        //[StringLength(50)]
        public Guid DeletedKey {

            get; set;

        }
        [Index("MonthYear", 4, IsUnique = true)]
        public int UserID { get; set; }

        public virtual ICollection<ScheduleDetail> ScheduleDetails { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now.Date;
        public ScheduleHeader()
        {

            Status = "Active";
        }
    }
}