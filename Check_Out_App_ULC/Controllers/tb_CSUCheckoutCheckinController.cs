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
            var view = db.tb_CSUCheckoutCheckin.OrderByDescending(s => s.CheckoutDate).Take(1000);
            if(SessionVariables.CurrentLocation.ToString() != "notset" && SessionVariables.CurrentLocation.ToString() != null)
            {
                view = db.tb_CSUCheckoutCheckin.Where(s => s.CheckoutLocationFK == SessionVariables.CurrentLocation.ToString()).OrderByDescending(s => s.CheckoutDate).Take(1000);
            }
            return View("Index", view);
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

