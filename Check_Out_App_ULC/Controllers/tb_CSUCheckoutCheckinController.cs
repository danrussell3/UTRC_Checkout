using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Check_Out_App_ULC.Models;
using System.Text;
using System.IO;

namespace Check_Out_App_ULC.Controllers
{
    public class tb_CSUCheckoutCheckinController : SecuredController
    {

        #region Constructors

        private Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();

        #endregion

        #region Public Functions

        public ActionResult Index()
        {
            var checkoutView =
            (from checkInCheckout in db.tb_CSUCheckoutCheckin
             join csuStudents in db.tb_CSUStudent on checkInCheckout.CSU_IDFK equals csuStudents.CSU_ID orderby checkInCheckout.CheckoutDate descending
             select new ViewModels.CkVw()
             {
                 CsuId = checkInCheckout.CSU_IDFK,
                 Ename = csuStudents.ENAME,
                 Name = csuStudents.FIRST_NAME + " " + csuStudents.LAST_NAME,
                 ItemUpc = checkInCheckout.ItemUPCFK,
                 CkOutLabTech = checkInCheckout.CheckoutLabTech,
                 CkOutDt = checkInCheckout.CheckoutDate.Value,
                 CkOutLoc = checkInCheckout.CheckoutLocationFK,
                 DueDate = checkInCheckout.DueDate,
                 LongTerm = checkInCheckout.isLongterm.Value
             });

            return View("Index", checkoutView);

        }
        // GET: tb_CSUCheckoutCheckin
        
            #region Default Functions
        // GET: tb_CSUCheckoutCheckin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_CSUCheckoutCheckin = db.tb_CSUCheckoutCheckin.Find(id);
            if (tb_CSUCheckoutCheckin == null)
            {
                return HttpNotFound();
            }
            return View(tb_CSUCheckoutCheckin);
        }

        // GET: tb_CSUCheckoutCheckin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tb_CSUCheckoutCheckin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CheckoutCheckinId,CSU_IDFK,ItemIDFK,CheckoutLabTech,CheckoutDate,CheckinLabTech,CheckinDate,CheckoutLocationFK,CheckinLocationFK")] tb_CSUCheckoutCheckin tb_CSUCheckoutCheckin)
        {
            if (ModelState.IsValid)
            {
                db.tb_CSUCheckoutCheckin.Add(tb_CSUCheckoutCheckin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tb_CSUCheckoutCheckin);
        }

        // GET: tb_CSUCheckoutCheckin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_CSUCheckoutCheckin = db.tb_CSUCheckoutCheckin.Find(id);
            if (tb_CSUCheckoutCheckin == null)
            {
                return HttpNotFound();
            }
            return View(tb_CSUCheckoutCheckin);
        }

        // POST: tb_CSUCheckoutCheckin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "CheckoutCheckinId,CSU_IDFK,ItemUPCFK,CheckoutLabTech,CheckoutDate,CheckinLabTech,CheckinDate,CheckoutLocationFK,CheckinLocationFK, isLate, isFined, isPaid")] tb_CSUCheckoutCheckin tb_CSUCheckoutCheckin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_CSUCheckoutCheckin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("isLate");
            }
            return View(tb_CSUCheckoutCheckin);
        }


        // GET: tb_CSUCheckoutCheckin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_CSUCheckoutCheckin = db.tb_CSUCheckoutCheckin.Find(id);
            if (tb_CSUCheckoutCheckin == null)
            {
                return HttpNotFound();
            }
            return View(tb_CSUCheckoutCheckin);
        }

        // POST: tb_CSUCheckoutCheckin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var tb_CSUCheckoutCheckin = db.tb_CSUCheckoutCheckin.Find(id);
            db.tb_CSUCheckoutCheckin.Remove(tb_CSUCheckoutCheckin);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion Default Function

        public ActionResult Reports()
        {
            var view = db.tb_CSUCheckoutCheckin;

            ViewBag.Message = TempData["Message"];
            return View("Reports", view);
        }

        public ActionResult UsageReports(List<ViewModels.QueryCheckoutsView> view = null)
        {
            
            return View("UsageReports", view);
        }

        public ActionResult TestEmails()
        {
            var view = db.tb_CSUCheckoutCheckin;

            ViewBag.Message = TempData["Message"];
            return View("TestEmails", view);
        }

       
            #region Old Search Functions

        public ActionResult historyUPC(string id)
        {
            var x = db.tb_CSUCheckoutCheckin.Where(s => s.ItemUPCFK == id);
            if (x != null)
            {
                ViewBag.Message = "You searched for " + id;
                return View("Index", x.ToList());
            }
            ViewBag.Message = "I'm given her all I've got Captian, but I can't find that item number!";
            return View();
        }

        //Searches Student Checkout History
        public ActionResult searchID(string id)
        {
            var x = db.tb_CSUCheckoutCheckin.Where(s => s.CSU_IDFK == id);
            if (x != null)
            {
                ViewBag.Message = "You searched for " + id;
                return View("Index", x.ToList());
            }
            ViewBag.Message = "KHAAAAAANNNNN!!!!! was not found, and neither was that student #";
            return View();
        }
        public ActionResult searchDate(DateTime? searchDate)
        {

            try
            {
                var x = db.tb_CSUCheckoutCheckin.Where(s => DbFunctions.TruncateTime(s.CheckoutDate) == searchDate);
                if (x != null)
                {
                    return View("Index", x.ToList());
                }
                ViewBag.Message = "Did you enter a Saturday or a Sunday?";
                return View();
            }
            catch
            {
                ViewBag.Message = "Are you trying to break it?!?! Just kidding Invalid date";
                return View("Index");
            }
        }

        #endregion UnUsed Functions

        //Show overdue items
        public ActionResult LateView()
        {
            IQueryable<ViewModels.CkVw> lateView = GetLateView();
            ViewBag.Message = TempData["Message"];
            return View("LateView", lateView);
        }
        public ActionResult FineView()
        {
            IQueryable<ViewModels.CkVw> lateView = GetFineView();
            ViewBag.Message = TempData["Message"];
            return View("LateView", lateView);
        }


        //show items with a manual fee over-ride 

        public ActionResult LongTermCheckout()
        {
            var longTerm = db.tb_CSUCheckoutCheckin.Where(s => s.isLongterm == true);
            return View("Longterm Checkouts", longTerm);
        }
     
        public ActionResult FeeWaive(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var noFee = db.tb_CSUCheckoutCheckin.Find(id);
            if (noFee == null)
            {
                return HttpNotFound();
            }
            noFee.isFined = false;
            db.Entry(noFee).State = EntityState.Modified;
            db.SaveChanges();
            var s = noFee.CSU_IDFK;
            var pa = db.tb_CSUStudent.FirstOrDefault(se => se.CSU_ID == s);
            if (pa == null) return RedirectToAction("LateView");
            var sfn = pa.FIRST_NAME;
            var sln = pa.LAST_NAME;
            var sa = noFee.ItemUPCFK;
            TempData["Message"] = s + " " + sfn + " " + sln + " checkout of " + sa + " Fee Waived!";
            return RedirectToAction("LateView");

        }

        //Marks isFined as true, button on isLate view
        public ActionResult ApplyFine(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var noFee = db.tb_CSUCheckoutCheckin.Find(id);
            if (noFee == null)
            {
                return HttpNotFound();
            }


            var s = noFee.CSU_IDFK;
            var pa = db.tb_CSUStudent.FirstOrDefault(se => se.CSU_ID == s);
            var sfn = pa.FIRST_NAME + " " + pa.LAST_NAME;
            var sa = noFee.ItemUPCFK;
            noFee.isFined = true;
            db.Entry(noFee).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Message"] = s + " " + sfn + " checkout of " + sa + " Fine Applied!";
            return RedirectToAction("LateView");
        }

        public ActionResult QueryCheckouts(DateTime startDate, DateTime endDate, string location, string action, string queryType)
        {
            List<ViewModels.QueryCheckoutsView> results = new List<ViewModels.QueryCheckoutsView>();

            if (queryType == "date")
            {
                results = QueryCheckoutsByDate(startDate, endDate, location, action);
            }
            else if (queryType == "dayofweek")
            {
                results = QueryCheckoutsByDayOfWeek(startDate, endDate, location, action);
            }
            else if (queryType == "hour")
            {
                results = QueryCheckoutsByHour(startDate, endDate, location, action);
            }
            else if (queryType == "item")
            {
                results = QueryCheckoutsByItem(startDate, endDate, location, action);
            }

            ViewBag.queryType = queryType;
            return View("UsageReports", results);
        }

        // queries # of checkouts by date and location
        // result table is either returned for display or exported as an Excel file
        public List<ViewModels.QueryCheckoutsView> QueryCheckoutsByDate(DateTime startDate, DateTime endDate, string location, string action)
        {
            // get contents of tb_CSUCheckoutCheckin with date/loc parameters
            var checkouts = db.tb_CSUCheckoutCheckin.Where(m => m.CheckoutDate >= startDate && m.CheckoutDate <= endDate);

            List<ViewModels.QueryCheckoutsView> results = new List<ViewModels.QueryCheckoutsView>();

            foreach (var c in checkouts)
            {
                if (location=="All" || location==c.CheckoutLocationFK)
                {
                    bool containsDateAlready = results.Any(item => Convert.ToDateTime(item.CheckoutDate).Date == Convert.ToDateTime(c.CheckoutDate).Date);
                    if (containsDateAlready)
                    {
                        // increment date tally
                        foreach (var r in results)
                        {
                            if (Convert.ToDateTime(r.CheckoutDate).Date == Convert.ToDateTime(c.CheckoutDate).Date)
                            {
                                r.NumCheckouts++;
                            }
                        }
                    }
                    else
                    {
                        // add date to list and set the tally to 1
                        ViewModels.QueryCheckoutsView entry = new ViewModels.QueryCheckoutsView();
                        entry.CheckoutDate = Convert.ToDateTime(c.CheckoutDate);
                        entry.NumCheckouts = 1;
                        results.Add(entry);
                    }
                }
            }

            if (action == "excel")
            {
                List<ExportModels.ExportCheckoutsByDate> data = new List<ExportModels.ExportCheckoutsByDate>();

                foreach (var item in results)
                {
                    ExportModels.ExportCheckoutsByDate e = new ExportModels.ExportCheckoutsByDate();
                    e.CheckoutDate = Convert.ToDateTime(item.CheckoutDate).ToShortDateString();
                    e.NumberOfCheckouts = Convert.ToInt32(item.NumCheckouts);
                    data.Add(e);
                }
                
                var grid = new System.Web.UI.WebControls.GridView();
                grid.DataSource = data;
                grid.DataBind();
                Response.ClearContent();
                var fname = "CheckoutsByDate_" + location + ".xls";
                Response.AddHeader("content-disposition", "attachment; filename=" + fname);
                Response.ContentType = "application/vnd.ms-excel";
                StringWriter sw = new StringWriter();
                System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
                grid.RenderControl(htw);
                Response.Write(sw.ToString());
                Response.End();

                List<ViewModels.QueryCheckoutsView> emptyResults = new List<ViewModels.QueryCheckoutsView>();
                return emptyResults;
            }
            return results;
        }

        // queries # of checkouts by day of week and location
        // result table is either returned for display or exported as an Excel file
        public List<ViewModels.QueryCheckoutsView> QueryCheckoutsByDayOfWeek (DateTime startDate, DateTime endDate, string location, string action)
        {
            // get contents of tb_CSUCheckoutCheckin with date/loc parameters
            var checkouts = db.tb_CSUCheckoutCheckin.Where(m => m.CheckoutDate >= startDate && m.CheckoutDate <= endDate);

            List<ViewModels.QueryCheckoutsView> results = new List<ViewModels.QueryCheckoutsView>();

            foreach (var c in checkouts)
            {
                if (location == "All" || location == c.CheckoutLocationFK)
                {
                    bool containsDayAlready = results.Any(item => item.CheckoutDayOfWeek == Convert.ToDateTime(c.CheckoutDate).DayOfWeek.ToString());
                    if (containsDayAlready)
                    {
                        // increment date tally
                        foreach (var r in results)
                        {
                            if (r.CheckoutDayOfWeek == Convert.ToDateTime(c.CheckoutDate).DayOfWeek.ToString())
                            {
                                r.NumCheckouts++;
                            }
                        }
                    }
                    else
                    {
                        // add date to list and set the tally to 1
                        ViewModels.QueryCheckoutsView entry = new ViewModels.QueryCheckoutsView();
                        entry.CheckoutDayOfWeek = Convert.ToDateTime(c.CheckoutDate).DayOfWeek.ToString();
                        entry.NumCheckouts = 1;
                        results.Add(entry);
                    }
                }
            }

            var sortedResults = results.OrderByDescending(a => a.NumCheckouts).ToList();

            if (action == "excel")
            {
                List<ExportModels.ExportCheckoutsByDayOfWeek> data = new List<ExportModels.ExportCheckoutsByDayOfWeek>();

                foreach (var item in results)
                {
                    ExportModels.ExportCheckoutsByDayOfWeek e = new ExportModels.ExportCheckoutsByDayOfWeek();
                    e.CheckoutDayOfWeek = item.CheckoutDayOfWeek;
                    e.NumberOfCheckouts = Convert.ToInt32(item.NumCheckouts);
                    data.Add(e);
                }

                var grid = new System.Web.UI.WebControls.GridView();
                grid.DataSource = data;
                grid.DataBind();
                Response.ClearContent();
                var fname = "CheckoutsByDayOfWeek_" + location + ".xls";
                Response.AddHeader("content-disposition", "attachment; filename=" + fname);
                Response.ContentType = "application/vnd.ms-excel";
                StringWriter sw = new StringWriter();
                System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
                grid.RenderControl(htw);
                Response.Write(sw.ToString());
                Response.End();

                List<ViewModels.QueryCheckoutsView> emptyResults = new List<ViewModels.QueryCheckoutsView>();
                return emptyResults;
            }
            return results;
        }

        // queries # of checkouts by hour and location
        // result table is either returned for display or exported as an Excel file
        public List<ViewModels.QueryCheckoutsView> QueryCheckoutsByHour(DateTime startDate, DateTime endDate, string location, string action)
        {
            // get contents of tb_CSUCheckoutCheckin with date/loc parameters
            var checkouts = db.tb_CSUCheckoutCheckin.Where(m => m.CheckoutDate >= startDate && m.CheckoutDate <= endDate);

            List<ViewModels.QueryCheckoutsView> results = new List<ViewModels.QueryCheckoutsView>();

            foreach (var c in checkouts)
            {
                if (location == "All" || location == c.CheckoutLocationFK)
                {
                    bool containsHourAlready = results.Any(item => item.CheckoutHour == Convert.ToDateTime(c.CheckoutDate).Hour);
                    if (containsHourAlready)
                    {
                        // increment hour tally
                        foreach (var r in results)
                        {
                            if (r.CheckoutHour == Convert.ToDateTime(c.CheckoutDate).Hour)
                            {
                                r.NumCheckouts++;
                            }
                        }
                    }
                    else
                    {
                        // add hour to list and set the tally to 1
                        ViewModels.QueryCheckoutsView entry = new ViewModels.QueryCheckoutsView();
                        entry.CheckoutHour = Convert.ToDateTime(c.CheckoutDate).Hour;
                        entry.NumCheckouts = 1;
                        results.Add(entry);
                    }
                }
            }

            var sortedResults = results.OrderBy(a => a.CheckoutHour).ToList();

            if (action == "excel")
            {
                List<ExportModels.ExportCheckoutsByHour> data = new List<ExportModels.ExportCheckoutsByHour>();

                foreach (var item in sortedResults)
                {
                    ExportModels.ExportCheckoutsByHour e = new ExportModels.ExportCheckoutsByHour();
                    e.CheckoutHour = item.CheckoutHour.ToString();
                    e.NumberOfCheckouts = Convert.ToInt32(item.NumCheckouts);
                    data.Add(e);
                }

                var grid = new System.Web.UI.WebControls.GridView();
                grid.DataSource = data;
                grid.DataBind();
                Response.ClearContent();
                var fname = "CheckoutsByHour_" + location + ".xls";
                Response.AddHeader("content-disposition", "attachment; filename=" + fname);
                Response.ContentType = "application/vnd.ms-excel";
                StringWriter sw = new StringWriter();
                System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
                grid.RenderControl(htw);
                Response.Write(sw.ToString());
                Response.End();

                List<ViewModels.QueryCheckoutsView> emptyResults = new List<ViewModels.QueryCheckoutsView>();
                return emptyResults;
            }
            return sortedResults;
        }

        // queries # of checkouts by item and location
        // result table is either returned for display or exported as an Excel file
        public List<ViewModels.QueryCheckoutsView> QueryCheckoutsByItem(DateTime startDate, DateTime endDate, string location, string action)
        {
            // get contents of tb_CSUCheckoutCheckin with date/loc parameters
            var checkouts = db.tb_CSUCheckoutCheckin.Where(m => m.CheckoutDate >= startDate && m.CheckoutDate <= endDate);

            List<ViewModels.QueryCheckoutsView> results = new List<ViewModels.QueryCheckoutsView>();

            var numCheckedBackIn = 0;
            foreach (var c in checkouts)
            {
                if (location == "All" || location == c.CheckoutLocationFK)
                {
                    bool containsItemAlready = results.Any(item => item.ItemUpc == c.ItemUPCFK);
                    if (containsItemAlready)
                    {
                        foreach (var r in results)
                        {
                            if (r.ItemUpc == c.ItemUPCFK)
                            {
                                if (c.isLate == true)
                                {
                                    r.NumLateCheckouts++;
                                }
                                if (c.CheckinDate != null)
                                {
                                    numCheckedBackIn++;
                                    var duration = (TimeSpan)(c.CheckinDate - c.CheckoutDate);
                                    r.AvgCheckoutLength = ((r.AvgCheckoutLength * (numCheckedBackIn-1)) + duration.TotalMinutes) / numCheckedBackIn;
                                }
                                r.NumCheckouts++;
                            }
                        }
                    }
                    else
                    {
                        ViewModels.QueryCheckoutsView entry = new ViewModels.QueryCheckoutsView();
                        entry.ItemUpc = c.ItemUPCFK;
                        entry.NumCheckouts = 1;
                        if(c.isLate==true)
                        {
                            entry.NumLateCheckouts = 1;
                        }
                        else
                        {
                            entry.NumLateCheckouts = 0;
                        }
                        if (c.CheckinDate != null)
                        {
                            numCheckedBackIn++;
                            TimeSpan duration = (TimeSpan)(c.CheckinDate - c.CheckoutDate);
                            entry.AvgCheckoutLength = duration.TotalMinutes;
                        }
                        results.Add(entry);
                    }
                }
            }

            var sortedResults = results.OrderByDescending(a => a.NumCheckouts).ToList();

            if (action == "excel")
            {
                List<ExportModels.ExportCheckoutsByItem> data = new List<ExportModels.ExportCheckoutsByItem>();

                foreach (var item in sortedResults)
                {
                    ExportModels.ExportCheckoutsByItem e = new ExportModels.ExportCheckoutsByItem();
                    e.ItemUpc = item.ItemUpc;
                    e.NumberOfCheckouts = Convert.ToInt32(item.NumCheckouts);
                    e.NumberOfLateCheckouts = Convert.ToInt32(item.NumLateCheckouts);
                    e.AverageLengthOfCheckout = Convert.ToDouble(item.AvgCheckoutLength);
                    data.Add(e);
                }

                var grid = new System.Web.UI.WebControls.GridView();
                grid.DataSource = data;
                grid.DataBind();
                Response.ClearContent();
                var fname = "CheckoutsByItem_" + location + ".xls";
                Response.AddHeader("content-disposition", "attachment; filename=" + fname);
                Response.ContentType = "application/vnd.ms-excel";
                StringWriter sw = new StringWriter();
                System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
                grid.RenderControl(htw);
                Response.Write(sw.ToString());
                Response.End();

                List<ViewModels.QueryCheckoutsView> emptyResults = new List<ViewModels.QueryCheckoutsView>();
                return emptyResults;
            }
            return sortedResults;
        }



        #endregion

        #region Private Functions

        private IQueryable<ViewModels.CkVw> GetLateView()
        {
            return (from checkInCheckout in db.tb_CSUCheckoutCheckin.Where(s => s.isLate == true)
                join csuStudents in db.tb_CSUStudent on checkInCheckout.CSU_IDFK equals csuStudents.CSU_ID
                select new ViewModels.CkVw()
                {
                    CsuId = checkInCheckout.CSU_IDFK,
                    Ename = csuStudents.ENAME,
                    Name = csuStudents.FIRST_NAME + " " + csuStudents.LAST_NAME,
                    ItemUpc = checkInCheckout.ItemUPCFK,
                    CkOutLabTech = checkInCheckout.CheckoutLabTech,
                    CkOutDt = checkInCheckout.CheckoutDate.Value,
                    CkOutLoc = checkInCheckout.CheckoutLocationFK,
                    CkInDt = checkInCheckout.CheckinDate,
                    CkInLabTech = checkInCheckout.CheckinLabTech,
                    DueDate = checkInCheckout.DueDate.Value,
                    LongTerm = checkInCheckout.isLongterm.Value,
                    CkOutId = checkInCheckout.CheckoutCheckinId

                });
        }

        private IQueryable<ViewModels.CkVw> GetFineView()
        {
            return (from checkInCheckout in db.tb_CSUCheckoutCheckin.Where(s => s.isFined == true)
                join csuStudents in db.tb_CSUStudent on checkInCheckout.CSU_IDFK equals csuStudents.CSU_ID
                select new ViewModels.CkVw()
                {
                    CsuId = checkInCheckout.CSU_IDFK,
                    Ename = csuStudents.ENAME,
                    Name = csuStudents.FIRST_NAME + " " + csuStudents.LAST_NAME,
                    ItemUpc = checkInCheckout.ItemUPCFK,
                    CkOutLabTech = checkInCheckout.CheckoutLabTech,
                    CkOutDt = checkInCheckout.CheckoutDate.Value,
                    CkOutLoc = checkInCheckout.CheckoutLocationFK,
                    CkInDt = checkInCheckout.CheckinDate,
                    CkInLabTech = checkInCheckout.CheckinLabTech,
                    DueDate = checkInCheckout.DueDate.Value,
                    LongTerm = checkInCheckout.isLongterm.Value,
                    CkOutId = checkInCheckout.CheckoutCheckinId

                });
        }
        #endregion Private Functions

        #region Protected Functions

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

    }
}

