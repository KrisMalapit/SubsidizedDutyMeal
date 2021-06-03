using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models
{
    public class BiometricsData
    {
        public int ID { get; set; }
        public string EnrollNumber { get; set; }
        public string VerifyMode { get; set; }
        public string InOutMode { get; set; }
        public DateTime Date { get; set; }
        public string WorkCode { get; set; }
        public DateTime DownloadDate { get; set; }
    }
}