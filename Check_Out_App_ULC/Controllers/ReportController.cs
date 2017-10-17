using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Check_Out_App_ULC.Models;


namespace Check_Out_App_ULC.Controllers
{
    public class ReportController : Controller
    {
        #region Constructors
        private readonly EmailController email = new EmailController();
        private readonly Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();
        public tb_CSUStudent CsuStudent = new tb_CSUStudent();
        public tb_Reports reports = new tb_Reports();
        public StringBuilder Sb = new StringBuilder();
        public IList<tb_CSUStudent> Students;
        #endregion
        // GET: Report

        #region Public Functions



        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult EndOfDayReport(string notes)
        {
            Sb.Clear();
            bool isTest = false;
            if (notes == "teststring")
            {
                isTest = true;
                // notes = "This is a sample end of day report, but the data is real!";
            }

            //build HTML view with StringBuilder
            Sb.Append("<div>Report submitted by " + SessionVariables.CurrentUser.First_Name + " " +
                      SessionVariables.CurrentUser.Last_Name + " at " + DateTime.Now + "<br/>" + " from " +
                      SessionVariables.CurrentLocation
                      
            );

            // add user notes
            Sb.Append("<h2>Notes</h2><div border='1'>" + notes + "</div>");

            //turn html string builder into string
            var report = Sb.ToString();
            
            if (isTest)
            {
                EmailEndOfDayReport(true);
            }
            else
            {
                // store report, timestamp, and tech id to db for later compilation
                var techReport = new tb_Reports();
                techReport.CSU_ID = SessionVariables.CurrentUserId;
                techReport.TimeSubmitted = DateTime.Now;
                techReport.Report = report;
                db.tb_Reports.Add(techReport);
                db.SaveChanges();
                // email.EndOfDayReport(report);
            }

            return RedirectToAction("index", "Home");
        }

        public ActionResult EmailLateStudents()
        {
            var checkoutRecords = db.tb_CSUCheckoutCheckin.Where(m => m.CheckinDate == null && m.isLongterm != true);
            foreach (var rec in checkoutRecords)
            {
                CsuStudent = db.tb_CSUStudent.FirstOrDefault(m => m.CSU_ID == rec.CSU_IDFK);
                if (CsuStudent == null) continue;
                if (rec.CheckoutLocationFK != "TLT")
                {
                    email.LateEmail(CsuStudent, rec);
                }
                
            }
            ViewBag.Message = "Email Sent!";
            return View();
        }

        public ActionResult EmailLongtermDueStudents()
        {
            var longtermRecords = db.tb_CSUCheckoutCheckin.Where(m => m.CheckinDate == null && m.isLongterm == true);
            foreach (var rec in longtermRecords)
            {
                CsuStudent = db.tb_CSUStudent.FirstOrDefault(m => m.CSU_ID == rec.CSU_IDFK);
                if (CsuStudent == null) continue;
                
                if (rec.DueDate < DateTime.Now.AddDays(5))
                {
                    email.LongtermDue(CsuStudent, rec);
                }
            }
            ViewBag.Message = "Email Sent!";
            return View();
        }

        // Email end of day report to management
        public ActionResult EmailEndOfDayReport(bool isTest = false)
        {
            Sb.Clear();
            
            // pull the day's reports and compile into one report
            var todaysReports = db.tb_Reports.Where(m => System.Data.Entity.DbFunctions.TruncateTime(m.TimeSubmitted) == System.Data.Entity.DbFunctions.TruncateTime(DateTime.Now));
            foreach (var rec in todaysReports)
            {
                // add each tech report
                var rep = rec.Report;
                Sb.Append(rep + "<br/>");
            }
            
            //collect records
            var lscCheckouts = db.tb_CSUCheckoutCheckin.Where(m => m.CheckoutDate.Value.Day == DateTime.Now.Day && m.CheckoutLocationFK == "LSC");
            var bsbCheckouts = db.tb_CSUCheckoutCheckin.Where(m => m.CheckoutDate.Value.Day == DateTime.Now.Day && m.CheckoutLocationFK == "BSB");
            //var tltCheckouts = db.tb_CSUCheckoutCheckin.Where(m => m.CheckoutDate.Value.Day == DateTime.Now.Day && m.CheckoutLocationFK == "TLT");
            var checkoutRecords = db.tb_CSUCheckoutCheckin.Where(m => m.CheckinDate == null && m.isLongterm != true);
            var longtermRecords = db.tb_CSUCheckoutCheckin.Where(m => m.CheckinDate == null && m.isLongterm == true);

            Sb.Append("</div><div><h3> # of Checkouts </h3>" +
                      "<li>LSC Checkouts: " + lscCheckouts.Count() + "</li>" +
                      "<li>BSB Checkouts: " + bsbCheckouts.Count() + "</li>");



            // create html table of items not checked in when report is run, filter by location and longterm
            Sb.Append("<h1>LSC Items not returned </h1><table border='1px' WIDTH='50%'   CELLPADDING='4' CELLSPACING='3' bordercolor='red'> <thead> <tr> <th>First Name</th>" + "<th>Last Name</th>" + "<th>CSU ID</th>" + "<th>Item</th>" + "<th>Checkout Date</th><th>Due Date</th> </tr> </thead> <tbody>");
            foreach (var rec in checkoutRecords.Where(m => m.CheckoutLocationFK == "LSC"))
            {

                CsuStudent = db.tb_CSUStudent.FirstOrDefault(m => m.CSU_ID == rec.CSU_IDFK);

                if (CsuStudent == null) continue;

                var name = "<tr><td>" + CsuStudent.FIRST_NAME + " </td><td> " + CsuStudent.LAST_NAME + "</td><td>" + rec.CSU_IDFK + " </td><td> " + rec.ItemUPCFK + "</td><td>" + rec.CheckoutDate + "</td><td>" + rec.DueDate + " </td></tr> ";
                Sb.Append(name);
            }
            Sb.Append("<tr> <tbody> </table>");
            Sb.Append("<h1>BSB Items not returned </h1><table border='1px' WIDTH='50%'   CELLPADDING='4' CELLSPACING='3' bordercolor='red'> <thead> <tr> <th>First Name</th>" + "<th>Last Name</th>" + "<th>CSU ID</th>" + "<th>Item</th>" + "<th>Checkout Date</th><th>Due Date</th> </tr> </thead> <tbody>");
            foreach (var rec in checkoutRecords.Where(m => m.CheckoutLocationFK == "BSB"))
            {

                CsuStudent = db.tb_CSUStudent.FirstOrDefault(m => m.CSU_ID == rec.CSU_IDFK);

                if (CsuStudent == null) continue;

                var name = "<tr><td>" + CsuStudent.FIRST_NAME + " </td><td> " + CsuStudent.LAST_NAME + "</td><td>" + rec.CSU_IDFK + " </td><td> " + rec.ItemUPCFK + "</td><td>" + rec.CheckoutDate + "</td><td>" + rec.DueDate + " </td></tr> ";

                Sb.Append(name);
            }
            Sb.Append("<tr> <tbody> </table>");


            Sb.Append("<h1> Items in Longterm </h1><table border='1px' WIDTH='50%' CELLPADDING='4' CELLSPACING='3'> <thead> <tr> <th>First Name</th>" + "<th>Last Name</th>" +
                      "<th>CSU ID</th>" + "<th>Item</th>" + "<th>Checkout Date</th> <th>Due Date</th> </tr> </thead> <tbody> ");

            foreach (var rec in longtermRecords)
            {
                CsuStudent = db.tb_CSUStudent.FirstOrDefault(m => m.CSU_ID == rec.CSU_IDFK);

                if (CsuStudent == null) continue;
                var name = "<tr><td>" + CsuStudent.FIRST_NAME + " </td><td> " + CsuStudent.LAST_NAME + "</td><td>" + rec.CSU_IDFK + " </td><td> " + rec.ItemUPCFK + "</td><td>" + rec.CheckoutDate + "</td><td>" + rec.DueDate + " </td></tr> ";
                Sb.Append(name);
            }
            Sb.Append("<tr> <tbody> </table>");

            // convert StringBuilder to string
            var compiledReport = Sb.ToString();

            // email compiled report to management (will be routed to current user's email if isTest==true)
            email.EndOfDayReport(compiledReport, isTest);
            
            ViewBag.Message = "Email Sent!";
            return View();
        }

        /*public ActionResult testEmailLateStudents()
        {
            var checkoutRecords = db.tb_CSUCheckoutCheckin.Where(m => m.CheckinDate == null && m.isLongterm != true);
            foreach (var rec in checkoutRecords)
            {

                CsuStudent = db.tb_CSUStudent.FirstOrDefault(m => m.CSU_ID == rec.CSU_IDFK);
                if (CsuStudent == null) continue;
                CsuStudent.EMAIL_ADDRESS = "russell1@mail.colostate.edu";
                email.LateEmail(CsuStudent, rec);

            }
            ViewBag.Message = "Email Sent!";
            return View();
        }*/
    }
    #endregion Public Functions
}