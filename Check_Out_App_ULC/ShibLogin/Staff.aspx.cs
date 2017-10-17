using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using KeyCommAppMVC.Models;

namespace ShibLogin
{

    public partial class ShibLogin_Staff : System.Web.UI.Page
    {
        bool debugging = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            message.Text = "";
            /* WTF - on RamWeb, it's redirecting to http instead of https for this part?!??? */
            //        if(HttpContext.Current.Request.Url.Scheme!="https")
            //        {
            //            Response.Redirect("https://" + HttpContext.Current.Request.Url.Authority
            //+ HttpContext.Current.Request.ApplicationPath);
            //            return;
            //        }

            // Programmatically add a <meta> element to the Header
            HtmlMeta meta1 = new HtmlMeta();
            meta1.Name = "refresh";
            meta1.HttpEquiv = "refresh";
            meta1.Content = "5; url=" + HttpContext.Current.Request.Url.Scheme
    + "://"
    + HttpContext.Current.Request.Url.Authority
    + HttpContext.Current.Request.ApplicationPath + "/Login/Login";
            Page.Header.Controls.Add(meta1);
            Login_Link.NavigateUrl = HttpContext.Current.Request.Url.Scheme
    + "://"
    + HttpContext.Current.Request.Url.Authority
    + HttpContext.Current.Request.ApplicationPath + "/Login/Login";
            Panel_Login_reloc.Visible = true;
            Panel_Problem.Visible = false;

            SessionVariables.triedShiblogin = true;

            if (WebUser.getShibbolethUserName() != null && WebUser.getShibbolethUserName().Trim().Length > 0)
            {

                Page.Header.Controls.Remove(meta1);
                Panel_Login_reloc.Visible = false;
                Panel_Problem.Visible = true;

                WebUser w = new WebUser();
                w.generateSession(WebUser.getShibbolethUserName());

                Response.Redirect(HttpContext.Current.Request.ApplicationPath + "/AppReview/AppsToReview");

            }

            if (debugging)
            {
                Panel_Problem.Visible = true;

                message.Text += "Scheme: " + HttpContext.Current.Request.Url.Scheme + "<br /><hr /><br />";

                message.Text += "RamWeb EIDIRID : " + WebUser.getEIDIRID() + "<br /><hr /><br />";

                foreach (string k in Request.ServerVariables.AllKeys)
                {
                    message.Text += k + ":" + Request.ServerVariables[k] + "<br /><br />";
                }
            }


        }



    }


}
