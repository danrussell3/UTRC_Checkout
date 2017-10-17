using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Check_Out_App_ULC.Models;

namespace Check_Out_App_ULC.Controllers
{
    public class HomeController : Controller
    {
        #region Public Functions

        public ActionResult Index()
        {
            if (SessionVariables.CurrentUser != null && SessionVariables.locationSelected == false)
            {
                return View("Location");
            }
            else
            {
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "This program is dedicated towards improving the functionality of the Laptop Checkout team, by providing the resources necessary to perform the job exceptionally.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        public ActionResult localLogin()
        {
            if (Request.Url.Host.ToLower() != "localhost")
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult localLogin(string ENAME)
        {
            if (Request.Url.Host.ToLower() != "localhost")
            {
                return RedirectToAction("Index");
            }
            var currentUser = tb_CSULabTechs.tryLabTechSession(ENAME);

            if (currentUser != null)
            {
                //users logged in
                if (SessionVariables.CurrentUserLocation == "BSB")
                {
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
                return RedirectToAction("Index","tb_CSUCheckoutCheckin", null);
            }

            ViewData["message"] = "You're not a vaild user!";


            return View();
        }

        [HttpPost]
        public ActionResult changeLoc(string value)
        {
            // May need to wall off some locations from others (i.e. TLT users can't access BSB/LSC)

            if (SessionVariables.locationSelected == false)
            {
                SessionVariables.locationSelected = true;
            }

            switch (value)
            {
                case "BSB":
                    SessionVariables.CurrentLocation = SessionVariables.Location.BSB;
                    return Content("BSB");
                case "LSC":
                    SessionVariables.CurrentLocation = SessionVariables.Location.LSC;
                    var str = SessionVariables.CurrentLocation;
                    return Content("LSC");
                case "TLT":
                    SessionVariables.CurrentLocation = SessionVariables.Location.TLT;
                    return Content("TLT");
                case "notset":
                    SessionVariables.CurrentLocation = SessionVariables.Location.notset;
                    return Content("notset");
                default:
                    return Content("error");
            }
            //return Content("success");
        }


        public ActionResult Zoidberg()
        {
            return View("Zoidberg");
        }

        #endregion

    }
}