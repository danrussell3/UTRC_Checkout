using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Check_Out_App_ULC.Models
{

    public static class StringExt
    {
        #region Public Functions

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        #endregion
    }

    public static class SessionVariables
    {
        #region Public Functions

        public static string waiverCSUId
        {
            get
            {
                if (HttpContext.Current.Session["waiverCSUId"] == null)
                {
                    return null;
                }
                var v = (string)HttpContext.Current.Session["waiverCSUId"];
                return v;
            }
            set
            {
                HttpContext.Current.Session["waiverCSUId"] = value;
            }
        }

        public static string waiverUPC
        {
            get
            {
                if (HttpContext.Current.Session["waiverUPC"] == null)
                {
                    return null;
                }
                var v = (string)HttpContext.Current.Session["waiverUPC"];
                return v;
            }
            set
            {
                HttpContext.Current.Session["waiverUPC"] = value;
            }
        }

        public static string waiverUPC1
        {
            get
            {
                if (HttpContext.Current.Session["waiverUPC1"] == null)
                {
                    return null;
                }
                var v = (string)HttpContext.Current.Session["waiverUPC1"];
                return v;
            }
            set
            {
                HttpContext.Current.Session["waiverUPC1"] = value;
            }
        }

        public static string waiverUPC2
        {
            get
            {
                if (HttpContext.Current.Session["waiverUPC2"] == null)
                {
                    return null;
                }
                var v = (string)HttpContext.Current.Session["waiverUPC2"];
                return v;
            }
            set
            {
                HttpContext.Current.Session["waiverUPC2"] = value;
            }
        }

        // used to hold longterm checkout status while waiver is signed by students new to the system
        public static bool isLongterm
        {
            get
            {
                if (HttpContext.Current.Session["isLongterm"] == null)
                {
                    return false;
                }
                var v = (bool)HttpContext.Current.Session["isLongterm"];
                return v;
            }
            set
            {
                HttpContext.Current.Session["isLongterm"] = value;
            }
        }

        /*
        // used to hold waitlist status; true if all waitlist items are either checked out or reserved
        public static bool waitlistIsActive
        {
            get
            {
                if (HttpContext.Current.Session["waitlistIsActive"] == null)
                {
                    return false;
                }
                var v = (bool)HttpContext.Current.Session["waitlistIsActive"];
                return v;
            }
            set
            {
                HttpContext.Current.Session["waitlistIsActive"] = value;
            }
        }*/

        // used to hold longterm checkout status while waiver is signed by students new to the system
        public static DateTime? longtermDueDate
        {
            get
            {
                if (HttpContext.Current.Session["longtermDueDate"] == null)
                {
                    return null;
                }
                var v = (DateTime)HttpContext.Current.Session["longtermDueDate"];
                return v;
            }
            set
            {
                HttpContext.Current.Session["longtermDueDate"] = value;
            }
        }

        // indicates whether an initial location was selected
        public static bool locationSelected
        {
            get
            {
                if (HttpContext.Current.Session["locationSelected"] == null)
                {
                    return false;
                }
                var v = (bool)HttpContext.Current.Session["locationSelected"];
                return v;
            }
            set
            {
                HttpContext.Current.Session["locationSelected"] = value;
            }
        }
        
        public static tb_CSUCheckoutCheckin checkoutRecord
        {
            get
            {
                if (HttpContext.Current.Session["checkoutRecord"] == null)
                {
                    return null;
                }
                var v = (tb_CSUCheckoutCheckin)HttpContext.Current.Session["checkoutRecord"];
                return v;
            }
            set
            {
                HttpContext.Current.Session["checkoutRecord"] = value;
            }

        }
        public static tb_CSUCheckoutCheckin checkoutRecord1
        {
            get
            {
                if (HttpContext.Current.Session["checkoutRecord1"] == null)
                {
                    return null;
                }
                var v = (tb_CSUCheckoutCheckin)HttpContext.Current.Session["checkoutRecord1"];
                return v;
            }
            set
            {
                HttpContext.Current.Session["checkoutRecord1"] = value;
            }

        }
        public static tb_CSUCheckoutCheckin checkoutRecord2
        {
            get
            {
                if (HttpContext.Current.Session["checkoutRecord2"] == null)
                {
                    return null;
                }
                var v = (tb_CSUCheckoutCheckin)HttpContext.Current.Session["checkoutRecord2"];
                return v;
            }
            set
            {
                HttpContext.Current.Session["checkoutRecord2"] = value;
            }

        }

        public static bool triedShiblogin
        {
            get
            {
                if (HttpContext.Current.Session["triedShiblogin"] == null)
                {
                    return false;
                }
                var v = (bool)HttpContext.Current.Session["triedShiblogin"];
                return v;
            }
            set
            {
                HttpContext.Current.Session["triedShiblogin"] = value;
            }

        }

        public static tb_CSULabTechs CurrentUser
        {

            get
            {
                if (HttpContext.Current.Session["_CurrentUser"] == null)
                {
                    return null;
                }
                var curUser = (tb_CSULabTechs)HttpContext.Current.Session["_CurrentUser"];
                return curUser;
            }
            set
            {
                HttpContext.Current.Session["_CurrentUser"] = value;
            }

        }

        public static string CurrentUserId
        {

            get
            {
                if (CurrentUser != null)
                    return CurrentUser.ENAME;
                return null;
                //return (System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            }



        }

        public static string CurrentUserLocation
        {
            get
            {
                if (CurrentUser != null)
                    return CurrentUser.LocationId;
                return null;
            }
        }

        public enum Location { notset, LSC, BSB, TLT };
        public static Location CurrentLocation
        {

            get
            {
                if (HttpContext.Current.Session["_CurrentLocation"] == null)
                {
                    HttpContext.Current.Session["_CurrentLocation"] = Location.notset;
                }
                var curloc = (Location)HttpContext.Current.Session["_CurrentLocation"];
                return curloc;
            }
            set
            {
                HttpContext.Current.Session["_CurrentLocation"] = value;
            }

        }

        public static DateTime ItemDueDateTime
        {//create item due back time based on day of week and time of day.
            get
            {
                DateTime supposedTime;
                if (CurrentLocation == Location.LSC || CurrentLocation == Location.BSB || CurrentLocation == Location.notset)
                {

                    var tgif = DateTime.Now.DayOfWeek.ToString();

                    supposedTime = DateTime.Now.AddHours(6);
                    if (supposedTime > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 45, 00) && tgif != "Friday") // after 6pm
                    {
                        supposedTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 45, 00);
                        return supposedTime;
                    }
                    if (supposedTime > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 00, 00) && tgif == "Friday")
                    {
                        supposedTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 45, 00);
                        return supposedTime;
                    }
                }
                if (CurrentLocation == Location.TLT)
                {
                    supposedTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 22, 00,
                        00);

                    return supposedTime;
                }
                else
                {
                    supposedTime = DateTime.Now.AddHours(6);

                    var tgif = DateTime.Now.DayOfWeek.ToString();
                    if (supposedTime > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 45, 00) && tgif != "Friday") // after 6pm
                    {
                        supposedTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 45, 00);
                        return supposedTime;
                    }
                    if (supposedTime > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 00, 00) && tgif == "Friday")
                    {
                        supposedTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 45, 00);
                        return supposedTime;
                    }
                    return supposedTime;
                }
            }
        }
        public class feeEmail
        {
            //OutlookApp outlookApp = new OutlookApp();


        }
        public static string upcSV
        {
            get
            {
                if (HttpContext.Current.Session["upcSV"] == null)
                {
                    return null;
                }
                var v = (string)HttpContext.Current.Session["upcSV"];
                return v;
            }
            set
            {
                HttpContext.Current.Session["upcSV"] = value;
            }
        }

        //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
        //{
        //    public ApplicationDbContext()
        //        : base("DefaultConnection", throwIfV1Schema: false)
        //    {
        //    }

        //    public static ApplicationDbContext Create()
        //    {
        //        return new ApplicationDbContext();
        //    }
        //}

        #endregion
    }

}
