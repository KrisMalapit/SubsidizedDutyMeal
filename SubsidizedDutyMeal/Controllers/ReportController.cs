using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using SubsidizedDutyMeal.DAL;

namespace SubsidizedDutyMeal.Controllers
{
    public class ReportController : Controller
    {
        private SDMContext db = new SDMContext();
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        public DataTable PrintDailySummary(int ConcessionaireID, DateTime OrderDate)
        {
            string consName = "";
            //switch (ConcessionaireID)
            //{
            //    case 1:
            //        consName = "Canteen 1";
            //        break;
            //    case 2:
            //        consName = "Canteen 2";
            //        break;
            //    case 3:
            //        consName = "Canteen 3";
            //        break;
            //    default:
            //        break;
            //}
             
            consName = db.Concessionaires
                .Where(a => a.ID == ConcessionaireID)
                .Select(a => a.Name)
                .FirstOrDefault();




                var item = db.MealDetails.Where(a => a.MealHeaders.Status == "Active")
                   
                    .Where(a => a.MealHeaders.Date == OrderDate)
                    .Where(a => a.Status == "Active")


                  .Select(a => new {
                      MealName = a.Name
                          ,
                      a.Descriptions
                          ,
                      a.MealType
                          ,
                      Servings = a.NoServings
                      ,
                      Orders = db.OrderDetails.Where(b => b.Status != "Deleted").Where(b => b.MealDetailsID == a.ID).Count()
                      ,
                      Concessionaire = consName
                      ,
                      OrderDate,
                      a.MealHeaders.ConcessionaireID
                  });

            if (ConcessionaireID != 4) // 4 is administrator id
            {
                item = item.Where(a => a.ConcessionaireID == ConcessionaireID);
            }
           
            var m = ToDataTable(item.ToList());
                DataTable dt = m;

                return dt;
            


        }
        public DataTable PrintMealSummary(int ConcessionaireID,DateTime FromDate, DateTime ToDate, string Status,string CompanyCode)
        {
            DataTable dt;
            string consName = "";
            consName = db.Concessionaires.Where(a => a.ID == ConcessionaireID).Select(a => a.Name).FirstOrDefault();
            DateTime dateTime = DateTime.Now.Date;


            





                if (Status == "All")
                {
                var item = db.OrderDetails.Where(a => a.OrderHeaders.Status == "Active")
                                .Where(a => a.OrderHeaders.Date >= FromDate).Where(a => a.OrderHeaders.Date <= ToDate)
                                .Where(a => a.Status != "Deleted")
                                //.Where(a => a.MealDetails.MealHeaders.ConcessionaireID == ConcessionaireID)
                                .Select(a => new {
                                    OrderDate = a.OrderHeaders.Date
                                    ,
                                    Company = db.Employees.Where(b => b.EmployeeID == a.OrderHeaders.EmployeeID).Select(b => b.Companies.Code).FirstOrDefault()
                                    , Department = db.Employees.Where(b => b.EmployeeID == a.OrderHeaders.EmployeeID).Select(b => b.Departments.Name).FirstOrDefault()
                                    ,
                                    a.OrderHeaders.EmployeeID
                                    ,
                                    Name = db.Employees.Where(b => b.EmployeeID == a.OrderHeaders.EmployeeID).Select(b =>  b.LastName + ", " + b.FirstName).FirstOrDefault()
                                    ,
                                    MealName = a.MealDetails.Name
                                     ,
                                    a.MealDetails.MealType
                                        ,
                                    a.MealDetails.Descriptions
                                        ,
                                    a.OrderType

                                    ,
                                    Status = (db.Employees.Where(b => b.EmployeeID == a.OrderHeaders.EmployeeID).FirstOrDefault().Blocked == 1) ? (a.OrderHeaders.Date >= dateTime ? "Blocked" : a.Status) : a.Status
                                  ,
                                  Concessionaire = consName
                                  ,
                                  FromDate
                                  ,
                                  ToDate,
                                    a.MealDetails.MealHeaders.ConcessionaireID
                                });


                //var m = ToDataTable(item.OrderBy(a=>a.OrderDate).ThenByDescending(a => a.MealType).ThenBy(a=>a.Company).ToList());


                if (ConcessionaireID != 4) // 4 is administrator id
                {
                    item = item.Where(a => a.ConcessionaireID == ConcessionaireID);
                }



                DataTable m;
                if (string.IsNullOrEmpty(CompanyCode) || CompanyCode == "ALL")
                {
                    m = ToDataTable(item.ToList());
                }
                else
                {
                    m = ToDataTable(item.Where(a => a.Company == CompanyCode).ToList());
                }

                dt = m;
            }
            else
            {
              var item = db.OrderDetails.Where(a => a.OrderHeaders.Status == "Active")
                        .Where(a => a.OrderHeaders.Date >= FromDate).Where(a => a.OrderHeaders.Date <= ToDate)
                        .Where(a => a.Status == Status)
                        //.Where(a => a.MealDetails.MealHeaders.ConcessionaireID == ConcessionaireID)
                        .Select(a => new {
                     OrderDate = a.OrderHeaders.Date
                     ,
                     Company = db.Employees.Where(b => b.EmployeeID == a.OrderHeaders.EmployeeID).Select(b => b.Companies.Code).FirstOrDefault()
                                  ,
                     Department = db.Employees.Where(b => b.EmployeeID == a.OrderHeaders.EmployeeID).Select(b => b.Departments.Name).FirstOrDefault()
                                  ,
                     a.OrderHeaders.EmployeeID
                     ,
                     Name = db.Employees.Where(b => b.EmployeeID == a.OrderHeaders.EmployeeID).Select(b => b.LastName + ", " + b.FirstName).FirstOrDefault()

                     ,
                     MealName = a.MealDetails.Name
                      ,
                     a.MealDetails.MealType
                         ,
                     a.MealDetails.Descriptions
                         ,
                     a.OrderType

                     ,
                     a.Status
                     ,
                     Concessionaire = consName
                     ,
                     FromDate
                     ,
                     ToDate
                     ,
                            a.MealDetails.MealHeaders.ConcessionaireID
                        });

                //var m = ToDataTable(item.ToList());
                DataTable m;


                if (ConcessionaireID != 4) // 4 is administrator id
                {
                    item = item.Where(a => a.ConcessionaireID == ConcessionaireID);
                }


                if (string.IsNullOrEmpty(CompanyCode) || CompanyCode == "ALL")
                {
                    m = ToDataTable(item.ToList());
                }
                else
                {
                    m = ToDataTable(item.Where(a => a.Company == CompanyCode).ToList());
                }

                dt = m;
            }





      
           




            return dt;
        }
        public DataTable PrintStatement(int ConcessionaireID, DateTime FromDate, DateTime ToDate, string CompanyCode)
        {
            DataTable dt;
            string consName = "";
            consName = db.Concessionaires.Where(a => a.ID == ConcessionaireID).Select(a => a.Name).FirstOrDefault();
            string optr =  db.Concessionaires.Where(a => a.ID == ConcessionaireID).Select(a => a.Operator).FirstOrDefault();


            var item = db.OrderDetails.Where(a => a.OrderHeaders.Status == "Active")
                                .Where(a => a.OrderHeaders.Date >= FromDate).Where(a => a.OrderHeaders.Date <= ToDate)
                                .Where(a => a.Status == "Served").Where(a => a.MealDetails.MealHeaders.ConcessionaireID == ConcessionaireID)
                              
                                .Select(a => new {
                                    CompanyCode = db.Employees.Where(b => b.EmployeeID == a.OrderHeaders.EmployeeID).Select(b => b.Companies.Code).FirstOrDefault()
                                    //,CompanyName = db.Employees.Where(b => b.EmployeeID == a.OrderHeaders.EmployeeID).Select(b => b.Companies.Name).FirstOrDefault()
                                    ,Amount = 60

                                });
             



            var qry = item.GroupBy(a => a.CompanyCode).Select(a=> new {
                CompanyCode = a.Key,
                CompanyName = db.Companies.Where(b => b.Code == a.Key).FirstOrDefault().Name,
                Concessionaire = consName,
                Total = a.Sum(b=>b.Amount),
                FromDate,
                ToDate,
                Operator = optr
            });


     

                DataTable m;
                if (string.IsNullOrEmpty(CompanyCode) || CompanyCode == "ALL")
                {
                    m = ToDataTable(qry.ToList());
                }
                else
                {
                    m = ToDataTable(qry.Where(a => a.CompanyCode == CompanyCode).ToList());
                }
                var x  = qry.ToList();
                dt = m;

            return dt;
        }
        public DataTable PrintAvailedMeals(int ConcessionaireID, DateTime FromDate, DateTime ToDate)
        {
            DataTable dt;
            string consName = "";
            consName = db.Concessionaires.Where(a => a.ID == ConcessionaireID).Select(a => a.Name).FirstOrDefault();

            var item = db.OrderDetails.Where(a => a.OrderHeaders.Status == "Active")
                                .Where(a => a.OrderHeaders.Date >= FromDate)
                                .Where(a => a.OrderHeaders.Date <= ToDate)
                                .Where(a => a.Status != "Deleted")
                                .Where(a => a.MealDetails.MealHeaders.ConcessionaireID == ConcessionaireID)
                                .Select(a => new {
                                    a.OrderHeaders.EmployeeID,
                                    Served = (a.Status == "Served" ? 1 : 0),
                                    Pending =  (a.Status == "Pending" ? 1 : 0)

            });

            var www = item.ToList();

            var qry = item.GroupBy(a => a.EmployeeID)
                .Select(a => new {     
                EmployeeID = a.Key, 
                Total = a.Sum(b => b.Served) + a.Sum(b => b.Pending),
                Availed = a.Sum(b => b.Served),


                FromDate,
                ToDate,
                Concessionaire = consName

            });
            var xxx = qry.Select(a => new {

                a.EmployeeID,
                EmployeeName = (db.Employees.Where(b => b.EmployeeID == a.EmployeeID.ToString()).Count() > 0) ? db.Employees.Where(b => b.EmployeeID == a.EmployeeID.ToString()).Select(b => b.LastName + ", " + b.FirstName).FirstOrDefault() : "",
                a.Total ,
                a.Availed ,
                FromDate,
                ToDate,
                a.Concessionaire

            }).ToList();
            DataTable m;
            m = ToDataTable(xxx);
            dt = m;

            return dt;
        }
        public DataTable PrintFeedBackSummary(DateTime FromDate, DateTime ToDate)
        {
            string consName = "";

            //consName = db.Concessionaires.Where(a => a.ID == ConcessionaireID).Select(a => a.Name).FirstOrDefault();

            var item = db.Feedbacks
              .Where(a => a.DateCreated >= FromDate)
              .Where(a => a.DateCreated <= ToDate)
              .Where(a => a.Status == "Active")
              .Select(a => new {
                  a.EmployeeID,
                  Concessionaire = db.Employees.Where(b=>b.EmployeeID == a.EmployeeID).Select(b=>b.Concessionaires.Name).FirstOrDefault(),
                  Taste = (a.Taste == "1" ? "Failed" : a.Taste == "2" ? "Poor" : a.Taste == "3" ? "Fair" : a.Taste == "4" ? "Good" : "Excellent"),
                  NutritionalValue = (a.NutritionalValue == "1" ? "Failed" : a.NutritionalValue == "2" ? "Poor" : a.NutritionalValue == "3" ? "Fair" : a.NutritionalValue == "4" ? "Good" : "Excellent"),
                  PresentationAppearance = (a.PresentationAppearance == "1" ? "Failed" : a.PresentationAppearance == "2" ? "Poor" : a.PresentationAppearance == "3" ? "Fair" : a.PresentationAppearance == "4" ? "Good" : "Excellent"),
                  Attentiveness = (a.Attentiveness == "1" ? "Failed" : a.Attentiveness == "2" ? "Poor" : a.Attentiveness == "3" ? "Fair" : a.Attentiveness == "4" ? "Good" : "Excellent"),
                  Courtesy = (a.Courtesy == "1" ? "Failed" : a.Courtesy == "2" ? "Poor" : a.Courtesy == "3" ? "Fair" : a.Courtesy == "4" ? "Good" : "Excellent"),
                  Speed = (a.Speed == "1" ? "Failed" : a.Speed == "2" ? "Poor" : a.Speed == "3" ? "Fair" : a.Speed == "4" ? "Good" : "Excellent"),
                  Appearance = (a.Appearance == "1" ? "Failed" : a.Appearance == "2" ? "Poor" : a.Appearance == "3" ? "Fair" : a.Appearance == "4" ? "Good" : "Excellent"),
                  OverallService = (a.OverallService == "1" ? "Failed" : a.OverallService == "2" ? "Poor" : a.OverallService == "3" ? "Fair" : a.OverallService == "4" ? "Good" : "Excellent"),
                  Utensils = (a.Utensils == "1" ? "Failed" : a.Utensils == "2" ? "Poor" : a.Utensils == "3" ? "Fair" : a.Utensils == "4" ? "Good" : "Excellent"),
                  TablesChairs = (a.TablesChairs == "1" ? "Failed" : a.TablesChairs == "2" ? "Poor" : a.TablesChairs == "3" ? "Fair" : a.TablesChairs == "4" ? "Good" : "Excellent"),
                  Floor = (a.Floor == "1" ? "Failed" : a.Floor == "2" ? "Poor" : a.Floor == "3" ? "Fair" : a.Floor == "4" ? "Good" : "Excellent"),
                  Ambiance = (a.Ambiance == "1" ? "Failed" : a.Ambiance == "2" ? "Poor" : a.Ambiance == "3" ? "Fair" : a.Ambiance == "4" ? "Good" : "Excellent"),
                  Comments = (a.Comments == "1" ? "Failed" : a.Comments == "2" ? "Poor" : a.Comments == "3" ? "Fair" : a.Comments == "4" ? "Good" : "Excellent"),
                  a.DateCreated,
                  FromDate,
                  ToDate,

              });




            var xxx = item.ToList();
            var m = ToDataTable(item.ToList());
            DataTable dt = m;
            return dt;
        }
        public DataTable PrintConcessionaireSummary(DateTime FromDate, DateTime ToDate)
        {

            //consName = db.Concessionaires.Where(a => a.ID == ConcessionaireID).Select(a => a.Name).FirstOrDefault();

            var toD = ToDate;
            toD = toD.AddDays(1);

            var item = db.BiometricsDatas
              .Where(a => a.Date >= FromDate && a.Date < toD)

              .Select(a => new
            {
                a.ID
                ,
                a.EnrollNumber
                ,
                EmployeeName = db.Employees.Where(b => b.EmployeeID.Contains(a.EnrollNumber)).Select(c => (c.LastName + ", " + c.FirstName)).FirstOrDefault()
                ,
                Concessionaire = db.Employees.Where(b => b.EmployeeID.Contains(a.EnrollNumber)).Select(c => c.Concessionaires.Name).FirstOrDefault()
                ,TransactionDate = a.Date
                ,
                FromDate
                ,
                ToDate
              });



            int cnt = item.Count();
            var xxx = item.OrderBy(a=>a.ID).ThenBy(a=>a.EmployeeName).ToList();
            var m = ToDataTable(xxx);
            DataTable dt = m;
            return dt;
        }
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}