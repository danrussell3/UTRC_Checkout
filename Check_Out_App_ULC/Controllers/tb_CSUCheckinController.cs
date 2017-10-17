using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Check_Out_App_ULC.Models;

namespace Check_Out_App_ULC.Controllers
{
    public class tb_CSUCheckinController : SecuredController
    {

        #region Constructors

        private readonly Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();
        private readonly EmailController email = new EmailController();
        private tb_CSUStudent stuRec;

        #endregion

        #region Public Functions

        public ActionResult Index()
        {
            IQueryable<ViewModels.CkVw> checkinView = GetCheckInView();
            return View("Index", checkinView);
        }

        public ActionResult CheckinSave(tb_CSUCheckoutCheckin id)
        {
            try
            {
                // Checkin_Checkout_Entities tb = new Checkin_Checkout_Entities();
                #region CheckinDetails
                //this is to select the checkin record
                var checkIn = id;

                if (checkIn == null)
                {
                    ViewBag.Message = "There was an error checking that Item in";
                    return View("Index");
                }
                checkIn.CheckinDate = DateTime.Now;
                checkIn.CheckinLabTech = SessionVariables.CurrentUserId;
                checkIn.CheckinLocationFK = SessionVariables.CurrentUserLocation;

                #endregion

                //checks for a late return
                LateCheck(checkIn, out checkIn, out  stuRec);

                // resets isCheckedOut
                //var itemupc = stuRec.WAITLIST_RESERVED;
                var item = db.tb_CSULabInventoryItems.FirstOrDefault(s => s.ItemUPC == id.ItemUPCFK);
                item.isCheckedOut = null;

                //Save changes, send email
                db.Entry(checkIn).State = EntityState.Modified;
                db.SaveChanges();

                // send checkin email
                email.CheckinEmail(checkIn.isLate, checkIn.isFined, stuRec, checkIn);

                // if waitlist item is being checked in, update waitlist
                if (checkIn.isWaitlistHP==true || checkIn.isWaitlistMac==true)
                {
                    TempData["Message"] = checkIn.ItemUPCFK + " successfully checked in at " + DateTime.Now.ToString("g");
                    return RedirectToAction("WaitlistStatusCheck", "tb_LongtermWaitlist");
                }
                else
                {
                    IQueryable<ViewModels.CkVw> checkinView = GetCheckInView();
                    ViewBag.Message = checkIn.ItemUPCFK + " successfully checked in at " + DateTime.Now.ToString("g");
                    ModelState.Clear();
                    return View("Index", checkinView);
                }
            }
            catch
            {
                ViewBag.Message = "ERRORS! ALL THE ERRORS!!!";
                return View("Index");
            }
        }

        public ActionResult IdCheckIn(int? id)
        {
            try
            {
                if (id == null)
                {
                    ViewBag.Message = "Invalid Item number";
                    return RedirectToAction("Index");
                }
                //this is to select the checkin record by in
                var checkIn =
                    db.tb_CSUCheckoutCheckin.FirstOrDefault(m => m.CheckoutCheckinId == id && m.CheckinDate == null);

                if (checkIn == null)
                {
                    ViewBag.Message = "That item is already checked-in to inventory";
                    return View("Index");
                }
                return CheckinSave(checkIn);
            }
            catch
            {
                ViewBag.Message =
                    "You cannot live life without error, because life is error... and there was an error checking that Item in";
                return View("Index");
            }
        }

        public ActionResult ItemCheckIn(string item)
        {
            try
            {
                if (item == null)
                {
                    ViewBag.Message = "Invalid Item number";
                    return RedirectToAction("Index");
                }
                //this is to select the checkin record by in
                var checkIn = db.tb_CSUCheckoutCheckin.FirstOrDefault(m => m.ItemUPCFK == item && m.CheckinDate == null);

                if (checkIn == null)
                {
                    ViewBag.Message = "That item is already checked-in to inventory";
                    return View("Index");
                }
                return CheckinSave(checkIn);
                
            }
            catch
            {
                ViewBag.Message = "You cannot live life without error, because life is error... and there was an error checking that Item in";
                return View("Index");
            }
        }

        // GET: tb_CSUCheckin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tbCsuCheckoutCheckin = db.tb_CSUCheckoutCheckin.Find(id);
            if (tbCsuCheckoutCheckin == null)
            {
                return HttpNotFound();
            }
            return View(tbCsuCheckoutCheckin);
        }

        // POST: tb_CSUCheckin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CheckoutCheckinId,CSU_IDFK,ItemIDFK,CheckoutLabTech,CheckoutDate,CheckinLabTech,CheckinDate,CheckoutLocationFK,CheckinLocationFK")] tb_CSUCheckoutCheckin tb_CSUCheckoutCheckin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_CSUCheckoutCheckin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_CSUCheckoutCheckin);
        }

        // GET: tb_CSUCheckin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tbCsuCheckoutCheckin = db.tb_CSUCheckoutCheckin.Find(id);
            if (tbCsuCheckoutCheckin == null)
            {
                Session["Message"] = "There was an error checking that Item in";
                return View("Index");
            }
            return View(tbCsuCheckoutCheckin);
        }

        // POST: tb_CSUCheckin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var tbCsuCheckoutCheckin = db.tb_CSUCheckoutCheckin.Find(id);
            db.tb_CSUCheckoutCheckin.Remove(tbCsuCheckoutCheckin);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        #region Private Functions

        private IQueryable<ViewModels.CkVw> GetCheckInView()
        {
            return (from checkInCheckout in db.tb_CSUCheckoutCheckin.Where(s => s.CheckinDate == null && s.isLongterm != true)
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
                        DueDate = checkInCheckout.DueDate.Value,
                        LongTerm = checkInCheckout.isLongterm.Value,
                        CkOutId = checkInCheckout.CheckoutCheckinId
                    });
        }

        private void LateCheck(tb_CSUCheckoutCheckin checkin, out tb_CSUCheckoutCheckin checkIn, out tb_CSUStudent stuRec)
        {
            // assigns records
            checkIn = checkin;
            stuRec = db.tb_CSUStudent.FirstOrDefault(m => m.CSU_ID == checkin.CSU_IDFK);
            if (checkIn == null) return;
            if (stuRec == null) return;


            //checks for a late checkin by 6 hours.
            var fullStudentName = stuRec.FIRST_NAME + " " + stuRec.LAST_NAME;

            var lateCheck = checkIn.CheckinDate < checkIn.DueDate;
       
            if (lateCheck || checkIn.isLongterm == true) return;

            checkIn.isLate = true;
            ViewBag.Message = checkIn.ItemUPCFK + " is late, a warning will be issued to " + fullStudentName;

            Debug.Assert(checkIn.DueDate != null, "checkIn.DueDate != null");

            DateTime overdueCheck = checkIn.DueDate.Value.AddHours(18);
            var fineCheck = checkIn.CheckinDate < overdueCheck;
            if (fineCheck) return;
            checkIn.isFined = true;
            ViewBag.Message = checkIn.ItemUPCFK + " is late, a fine will be issued to " + fullStudentName;
        }

        #endregion

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

//Old Checkin Selector
//var details = (from checkInCheckout in db.tb_CSUCheckoutCheckin.Where(s => s.CheckinDate == null && s.CheckoutCheckinId == id)
//               join csuStudents in db.tb_CSUStudent on checkInCheckout.CSU_IDFK equals csuStudents.CSU_ID
//               join inventoryItems in db.tb_CSULabInventoryItems on checkInCheckout.ItemIDFK equals inventoryItems.ItemId
//               join itemsUpc in db.tb_CSULabInventoryItems on checkInCheckout.ItemUPCFK equals itemsUpc.ItemUPC
//               select new CheckoutDetails
//               {
//                   ENAME = csuStudents.ENAME,
//                   FIRST_NAME = csuStudents.FIRST_NAME,
//                   LAST_NAME = csuStudents.LAST_NAME,
//                   ItemDescription = inventoryItems.ItemDescription,
//                   ItemSerialNumber = inventoryItems.ItemSerialNumber,
//                   CheckoutCheckinId = checkInCheckout.CheckoutCheckinId,
//                   CSU_IDFK = checkInCheckout.CSU_IDFK,
//                   ItemIDFK = checkInCheckout.ItemIDFK,
//                   ItemUPCFK = checkInCheckout.ItemUPCFK,
//                   CheckinLabTech = SessionVariables.CurrentUserId,
//                   CheckoutLabTech = checkInCheckout.CheckoutLabTech,
//                   CheckoutDate = checkInCheckout.CheckoutDate,
//                   CheckoutLocationFK = checkInCheckout.CheckoutLocationFK,
//                   CheckinDate = DateTime.Now,
//                   DueDate = checkInCheckout.DueDate,
//                   CheckinLocationFK = SessionVariables.CurrentUserLocation

//               }).OrderByDescending(s => s.CheckoutDate).FirstOrDefault();
