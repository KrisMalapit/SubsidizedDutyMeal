using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [Index("IX_EmployeeID", 1, IsUnique = true)]
        [StringLength(50)]
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }

        public int EmploymentStatus { get; set; }
        public DateTime? ContractEndDate { get; set; }
        [Required]
        public int DepartmentID { get; set; }
        [Required]
        public int DivisionID { get; set; }
        [Required]
        public int CompanyID { get; set; }
        [Required]
        public int ConcessionaireID { get; set; }


        public virtual Department Departments { get; set; }
        public virtual Company Companies { get; set; }
        public virtual Division Divisions { get; set; }
        public virtual Concessionaire Concessionaires { get; set; }

        public int Blocked { get; set; }
        public string Status { get; set; }
        public Employee(){
            Status = "Active";
        }


    }
}