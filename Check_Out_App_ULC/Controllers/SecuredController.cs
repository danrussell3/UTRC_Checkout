using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Check_Out_App_ULC.Models;
namespace Check_Out_App_ULC.Controllers
{
    public class SecuredController : Controller
    {

        #region Protected Functions
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (Request.Cookies.Get("SecureSesh") != null && SessionVariables.CurrentUserId == null)
            {
                //regenerate session from cookies
                var currentUser = tb_CSULabTechs.regenerateSessionFromCookies();
               
            }

            if (Request.Params["message"] != null)
                ViewData["message"] = Request.Params["message"];

            try
            {
                Response.CacheControl = "no-cache";
                Response.AddHeader("Pragma", "no-cache");
                Response.Expires = -1;
            }
            catch { /*don't worry about it */}

        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
        
            if (!filterContext.HttpContext.Request.IsSecureConnection && filterContext.HttpContext.Request.Url.Host.ToLower() != "localhost" && filterContext.HttpContext.Request.Url.ToString().StartsWith("http:"))
            {
                var url = filterContext.HttpContext.Request.Url.ToString().Replace("http:", "htttps:");
                filterContext.Result = new RedirectResult(url);
            }
            else if (SessionVariables.CurrentUserId == null) // person is not logged in, this will take them to the Home/Index
            {
                if (Request.Url.Host.ToLower() == "localhost")
                    RedirectResultInApp(filterContext, "Home/localLogin");
                RedirectResultInApp(filterContext, "shiblogin");
            }
            // Ensure the prequalification page has been visited.
            else if (!SessionVariables.CurrentUser.UserRights && filterContext.RouteData.Values["Controller"].ToString().ToLower().Contains( "admin" ))
            {
                RedirectResultInApp(filterContext, "Home/Index");
            }

           

            base.OnActionExecuting(filterContext);
        }

        #endregion

        #region Public Functions

        public static void RedirectResultInApp(ActionExecutingContext filterContext, string controllerAndActionString)
        {
            var disableCache = true;
            if (filterContext.HttpContext.Response.StatusCode == 200)
            {
                filterContext.HttpContext.Response.StatusCode = 301;
                var Url = (System.Web.VirtualPathUtility.ToAbsolute("~") + "/").Replace("//", "/") + controllerAndActionString;
                if (Url.ToLower().Contains("secure"))
                {
                    Url = Url.Replace("http:", "https:");
                }
                filterContext.Result = new RedirectResult(Url, true);
                //disableCache = false;
            }

            if (disableCache)
            {
                filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
                filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.HttpContext.Response.Cache.SetNoStore();
            }


        }

        #endregion
    }
}
