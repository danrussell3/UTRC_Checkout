using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Check_Out_App_ULC.Models;
using System.Web.Mvc;

namespace Check_Out_App_ULC.Controllers.Api
{
    public class SlingController : ApiController
    {
        [System.Web.Http.Route("Api/Sling/SlingLogin")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Mvc.HttpPost]
        public IHttpActionResult SlingLogin()
        {
            var result = Sling.Login("", "", "", "");
            return Ok(result);
        }

        [System.Web.Http.Route("Api/Sling/SlingGetArticles")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Mvc.HttpGet]
        public List<ViewModels.SlingArticlesView> SlingGetArticles(string channel)
        {
            //Sling s = new Sling();
            var result = Sling.GetArticles(channel);
            return result;
        }
    }
}
