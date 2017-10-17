using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Check_Out_App_ULC.Models;

namespace ShibLogin
{

    public partial class ShibLogin_Default : System.Web.UI.Page
    {
        bool debugging = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            message.Text = "";
            if (!Request.IsSecureConnection && Request.Url.Host.ToLower() != "localhost" && (Request.Params["HTTP_X_FORWARDED_ISSECURE"] == null || Request.Params["HTTP_X_FORWARDED_ISSECURE"].ToString().ToLower() != "true"))
            {
                var url = Request.Url.ToString().Replace("http:", "https:");
                Response.Redirect(url);
                return;
            }

            var scheme = HttpContext.Current.Request.Url.Scheme;
            if (Request.Url.Host.ToLower() == "localhost")
                scheme = "http";
            else
                scheme = "https";

            var returnLocation = scheme
                + "://"
                + HttpContext.Current.Request.Url.Authority
                + HttpContext.Current.Request.ApplicationPath + "/" + "Home/Index";
            if (Request.Params["returnLocation"] != null)
            {
                returnLocation = scheme
                    + "://"
                    + HttpContext.Current.Request.Url.Authority
                    + HttpContext.Current.Request.ApplicationPath + "/" + Request.Params["returnLocation"];
            }

            // Programmatically add a <meta> element to the Header
            var meta1 = new HtmlMeta();
            meta1.Name = "refresh";
            meta1.HttpEquiv = "refresh";
            meta1.Content = "0; url=" + returnLocation;

            if (!debugging)
                Page.Header.Controls.Add(meta1);

            Login_Link.NavigateUrl = returnLocation;


            Panel_Login_reloc.Visible = true;
            Panel_Problem.Visible = false;


            SessionVariables.triedShiblogin = true;

            // Custom code to allow impersonation of a student trying to apply.//
            if (Request.Params["alias"] != null && (Request.Url.Host.ToLower() == "localhost"))
            {
                Page.Header.Controls.Remove(meta1);
                Panel_Login_reloc.Visible = true;
                Panel_Problem.Visible = false;

                SessionVariables.CurrentUser = tb_CSULabTechs.tryLabTechSession(Request.Params["alias"]); // may be null if user eid not matched
            }
            // "normal" check for if the user is logged in and shibolleth applied.  If shib is not active, will skip
            else if (tb_CSULabTechs.getShibbolethUserName() != null && tb_CSULabTechs.getShibbolethUserName().Trim().Length > 0)
            {

                Page.Header.Controls.Remove(meta1);
                Panel_Login_reloc.Visible = true;
                Panel_Problem.Visible = false;

                SessionVariables.CurrentUser = tb_CSULabTechs.tryLabTechSession(tb_CSULabTechs.getShibbolethUserName()); // may be null if user eid not matched
            } // may be null if user eid not matched

        
            
            if(SessionVariables.CurrentUser!=null)
            {
                //successful login
                if (SessionVariables.CurrentUserLocation == "BSB") {
                    SessionVariables.CurrentLocation = SessionVariables.Location.BSB;
                }
                else if (SessionVariables.CurrentUserLocation == "LSC")
                {
                    SessionVariables.CurrentLocation = SessionVariables.Location.LSC;
                }
                else if (SessionVariables.CurrentUserLocation == "TLT")
                {
                    SessionVariables.CurrentLocation = SessionVariables.Location.TLT;
                }
                else
                {
                    SessionVariables.CurrentLocation = SessionVariables.Location.notset;
                }

                if (!debugging)
                    Response.Redirect(returnLocation);
            }
            else
            {
                //eid not matched

                Panel_Login_reloc.Visible = false;
                Panel_Problem.Visible = true;

                message.Text = "You have logged in successfully but your eid was not matched.";

                //returnLocation = scheme
                //+ "://"
                //+ HttpContext.Current.Request.Url.Authority
                //+ HttpContext.Current.Request.ApplicationPath + "/" + "Login/eIDError?redir=" + Request.Params["returnLocation"];

                //if(!debugging)
                   // Response.Redirect(returnLocation);
            }

            if (debugging)
            {
                Panel_Problem.Visible = true;


                message.Text += "Scheme: " + HttpContext.Current.Request.Url.Scheme + "<br /><hr /><br />";

                message.Text += "Current User: " + ( SessionVariables.CurrentUser==null?"Not Matched":(SessionVariables.CurrentUser.First_Name + " " + SessionVariables.CurrentUser.Last_Name) ) + "<br /><hr /><br />";
                message.Text += "Shibolath User = "+ (tb_CSULabTechs.getShibbolethUserName()) + "<br /><hr /><br />";

                foreach (var k in Request.ServerVariables.AllKeys)
                {
                    message.Text += k + ":" + Request.ServerVariables[k] + "<br /><br />";
                }
            }
        }



    }


}
