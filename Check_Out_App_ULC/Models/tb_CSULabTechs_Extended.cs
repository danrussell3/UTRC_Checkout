using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Check_Out_App_ULC.Models
{
    public partial class tb_CSULabTechs  
    {

        public bool isManager => this.ManagerRights.HasValue;

        override public string ToString()
        {
            var formatOut = "";
            formatOut += "User Name: " + this.First_Name + " " + this.Last_Name;
            formatOut += "<br/>CSUID: " + this.CSU_ID;
            formatOut += "<br/>eID: " + this.ENAME;
            formatOut += "<br/>UserID: " + this.UserID;
            return formatOut + "<br/>";
        }
        public string FullName
        {
            get
            {
                return First_Name + " " + Last_Name;
            }
        }



        public void generateSession()
        {

                SessionVariables.CurrentUser = this;

                var usercook = new HttpCookie("SecureSesh", SessionVariables.CurrentUserId);
                usercook.Expires = DateTime.Now.AddMinutes(360);

                try
                {
                    System.Web.HttpContext.Current.Response.Cookies.Add(usercook);
                }
                catch { /*nothing*/ }
            

        }

        public static tb_CSULabTechs tryLabTechSession(string ENAME)
        {
         //   try
         //   {
                if (ENAME.Length != 0)
                {
                    var db = new Checkin_Checkout_Entities();
                    var w = db.tb_CSULabTechs.FirstOrDefault(lt => lt.ENAME == ENAME && !lt.DisabledDateTime.HasValue);
                    if (w != null)
                    {
                        w.generateSession();
                    }
                    return w;

                }
           // }
          //  catch { }
            return null;
        }

        public static tb_CSULabTechs regenerateSessionFromCookies()
        {
            try
            {
                var cookieVal = System.Web.HttpContext.Current.Request.Cookies.Get("SecureSesh").Value;
                if (cookieVal.Length != 0)
                {
                    var db = new Checkin_Checkout_Entities();
                    var w = db.tb_CSULabTechs.FirstOrDefault(lt => lt.ENAME == cookieVal && !lt.DisabledDateTime.HasValue);
                    if (w != null)
                    {
                        w.generateSession();
                    }
                    return w;

                }
            }
            catch { }
            return null;
        }

        public static void LogoutUser()
        {
            var usercook = new HttpCookie("SecureSesh", "");
            usercook.Expires = DateTime.Now;

            try
            {
                System.Web.HttpContext.Current.Response.Cookies.Add(usercook);
            }
            catch { /*nothing*/ }

            System.Web.HttpContext.Current.Session.Abandon();
        }


        public static string getShibbolethUserName()
        {
            if (HttpContext.Current.Request.ServerVariables["HTTP_COLOSTATEEDUPERSONEID"] == null || HttpContext.Current.Request.ServerVariables["HTTP_COLOSTATEEDUPERSONEID"].ToString().Length == 0)
            {
                return null;
            }
            else
            {
                return HttpContext.Current.Request.ServerVariables["HTTP_COLOSTATEEDUPERSONEID"].ToString();
            }
        }


        public static int getEIDIRID()
        {
            if (HttpContext.Current.Request.Cookies["EIDENTITYWEBAUTHEIDIRID"] == null || HttpContext.Current.Request.Cookies["EIDENTITYWEBAUTHEIDIRID"].Value.ToString().Length == 0)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(HttpContext.Current.Request.Cookies["EIDENTITYWEBAUTHEIDIRID"].Value);
            }
        }


    }
}