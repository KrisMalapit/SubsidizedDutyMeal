using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubsidizedDutyMeal.Models
{
    public class ScheduleMatrix
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index("IX_Code", 1, IsUnique = true)]
        [StringLength(20)]
        public string Code { get; set; }
        //[Index("IX_Description", 1, IsUnique = true)]
        [StringLength(100)]
        public string Description { get; set; }

        [StringLength(10)]
        public string MealType { get; set; }
        public string Status { get; set; }
        public ScheduleMatrix() {
            Status = "Active";
}
    }
}