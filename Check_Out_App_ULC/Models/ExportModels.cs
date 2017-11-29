using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;


namespace Check_Out_App_ULC.Models
{
    public class ExportModels
    {
        public class ExportCheckoutsByDate
        {
            public string CheckoutDate { get; set; }
            public int NumberOfCheckouts { get; set; }
        }

        public class ExportCheckoutsByDayOfWeek
        {
            public string CheckoutDayOfWeek { get; set; }
            public int NumberOfCheckouts { get; set; }
        }

        public class ExportCheckoutsByHour
        {
            public string CheckoutHour { get; set; }
            public int NumberOfCheckouts { get; set; }
        }

        public class ExportCheckoutsByItem
        {
            public string ItemUpc { get; set; }
            public int NumberOfCheckouts { get; set; }
            public int NumberOfLateCheckouts { get; set; }
            public double AverageLengthOfCheckout { get; set; }
        }
    }
}