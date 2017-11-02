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
            var result = Sling.Login("russell1@colostate.edu", "denali", "", "");
            return Ok(result);
        }

        [System.Web.Http.Route("Api/Sling/SlingGetAnnouncements")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Mvc.HttpGet]
        public IHttpActionResult SlingGetAnnouncements()
        {
            var result = Sling.GetAnnouncements();
            return Ok(result);
        }

        [System.Web.Http.Route("Api/Sling/SlingGetArticles")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Mvc.HttpGet]
        public IHttpActionResult SlingGetArticles()
        {
            var result = Sling.GetArticles();
            return Ok(result);
        }
    }
}
