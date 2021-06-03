using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.Models
{
    public class Utilities
    {
    }
    public enum EnumRate
    {
        Excellent = 5,
        Good = 4,
        Fair = 3,
        Poor = 2,
        Failed = 1
    }
    public class JsonArray
    {
        public string message { get; set; }
        public string status { get; set; }
        public int num1 { get; set; }
        public int num2 { get; set; }
        public int num3 { get; set; }
        public int num4 { get; set; }
        public int num5 { get; set; }
        public string param1 { get; set; }
        public string param2 { get; set; }
        public string param3 { get; set; }
        public string param4 { get; set; }
        public string param5 { get; set; }

    }
}