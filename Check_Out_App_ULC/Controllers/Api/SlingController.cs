using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//using System.Net.Http;
using System.Web.Http;
using Check_Out_App_ULC.Models;
using System.Web.Mvc;

namespace Check_Out_App_ULC.Controllers.Api
{
    public class SlingController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Mvc.HttpPost]
        public void SlingLogin()
        {
            Sling.Login("russell1@colostate.edu", "Waygoodaphj075hmu!", "", "");
            
        }
        
    }
}
