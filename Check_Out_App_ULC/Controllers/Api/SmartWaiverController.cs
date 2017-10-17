using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Check_Out_App_ULC.Models;
using Microsoft.Ajax.Utilities;

/*	
    Label

    UTRC_Checkout

 
    API Key for V3:

4286d68bf9a45da3230cdabc14103969-421758

    API Key for V4:

1358f518a45966dfca6484341e46b346

    Important: Save this key and only share with those you trust.
You won't be able to view this key again so save it carefully.
     
     
*/
namespace Check_Out_App_ULC.Controllers.Api
{
    public class SmartWaiverController : ApiController
    {
        //private static string smartKey = "1358f518a45966dfca6484341e46b346";

        

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional
                }
            );

            // Load SmartWaiver receiver
            
        }

        //public IEnumerable<SmartWaiver> GetAllWaivers()
        //{
        //    return waivers;
        //}

   
        //public IHttpActionResult GetWaiver(int id)
        //{
        //    var waiver = waivers.FirstOrDefault((p) => p.Id == id);
        //    if (waiver == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(waiver);
        //}
    }
}
