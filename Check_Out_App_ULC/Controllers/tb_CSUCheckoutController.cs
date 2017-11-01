using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Check_Out_App_ULC.Models;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;

namespace Check_Out_App_ULC.Controllers
{
    public class tb_CSUCheckoutController : SecuredController
    {
        #region Contstructors

        private readonly Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();
        readonly EmailController email = new EmailController();
        // GET: tb_CSUCheckout
        #endregion

        #region Public Functions
        
        public ActionResult Index()
        {
            var checkoutView =
            (from checkInCheckout in db.tb_CSUCheckoutCheckin.Where(s => s.CheckinDate == null && s.isLongterm != true)
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
                    DueDate = checkInCheckout.DueDate,
                    LongTerm = checkInCheckout.isLongterm.Value
                });

            return View("Index", checkoutView);
        }
        // GET: tb_CSUCheckout/Details/5
        public ActionResult Details(int? id)
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

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(string csuId, string upc, string upc1, string upc2)
        {
            try
            {
                var view = new ViewModels();
                if (csuId.Length > 9)
                {
                    //Shortens the scanned CSU-ID to 9 characters
                    csuId = csuId.Truncate(9);

                }
                if (view.IsBannedUser(csuId))
                {
                    ViewBag.Message = "That user has been banned from checkout!" + csuId;
                    return View("Index");
                }
             
                // Checks for Student record

                var csuStudent = GetOrMigrateStudent(csuId);

                //Checks for signed Waiver
                if (WaiverCheck(csuStudent) != true)
                {
                    SessionVariables.waiverCSUId = csuId;
                    SessionVariables.waiverUPC = upc;
                    SessionVariables.waiverUPC1 = upc1;
                    SessionVariables.waiverUPC2 = upc2;
                    ViewBag.Message = "Records indicate this user has not signed a waiver";
                    return View("Waiver", csuStudent);
                }

                //checks for UPC Entry
                var upcs = new List<string> { upc, upc1, upc2 };
                upcs = GetValidUpcs(upc, upc1, upc2);

                if (upcs.Count == 0)
                {
                    ViewBag.Message = "No UPC entered, try again " + csuId;
                    return View("Index");
                }

                if (upcs.Distinct().Count() != upcs.Count)
                {
                    ViewBag.Message = "There are duplicate UPCs, try again" + csuId;
                    return View("Index");
                }

                //Item Check
                var stuFirst = csuStudent.FIRST_NAME + " " + csuStudent.LAST_NAME;
                var checkoutRecord = new tb_CSUCheckoutCheckin();
                try
                {
                    foreach (var item in upcs)
                    {
                        var itemCheckout = db.tb_CSULabInventoryItems.FirstOrDefault(s => s.ItemUPC == item);
                        var checkedOut = db.tb_CSUCheckoutCheckin.FirstOrDefault(s => s.ItemUPCFK == item && s.CheckinDate == null);

                        if (itemCheckout == null)
                        {
                            ViewBag.Message = "Item #1 " + item + " number not found";
                            return View("Index");
                        }
                        if (checkedOut != null)
                        {
                            ViewBag.Message = "Our Records indicate that item" + item + " is checked-out, please check the item in first! Copy the CSU ID first though so you don't have to ask for the ID back " + csuId;
                            return View("Index");
                        }
                        //build record
                        checkoutRecord.CSU_IDFK = csuId;
                        checkoutRecord.ItemUPCFK = item;
                        checkoutRecord.ItemIDFK = itemCheckout.ItemId;
                        checkoutRecord.CheckoutLabTech = SessionVariables.CurrentUserId;
                        checkoutRecord.CheckoutDate = DateTime.Now;
                        checkoutRecord.CheckoutLocationFK = itemCheckout.ItemLocationFK;
                        checkoutRecord.DueDate = SessionVariables.ItemDueDateTime;
                        db.tb_CSUCheckoutCheckin.Add(checkoutRecord);
                        db.SaveChanges();
                        //email student
                        email.CheckoutEmail(csuStudent, checkoutRecord);
                    }
                }
                catch
                {
                    ViewBag.Message = "There was an error during check-out, please try again.";
                    return View();
                }
                var checkoutView =
                (from checkInCheckout in db.tb_CSUCheckoutCheckin.Where(s => s.CheckinDate == null && s.isLongterm != true)
                    join csuStudents in db.tb_CSUStudent on checkInCheckout.CSU_IDFK equals csuStudents.CSU_ID
                    select new ViewModels.CkVw()
                    {
                        CsuId = checkInCheckout.CSU_IDFK,
                        Ename = csuStudents.ENAME,
                        Name = csuStudents.FIRST_NAME + " " + csuStudents.LAST_NAME,
                        ItemUpc = checkInCheckout.ItemUPCFK,
                        CkOutLabTech = checkInCheckout.CheckoutLabTech,
                        CkOutDt = checkInCheckout.CheckoutDate.Value,
                    });

                ViewBag.Message = stuFirst + " " + string.Join(" and ", upcs) + " due back at " + SessionVariables.ItemDueDateTime;
                ModelState.Clear();
                return View("Index", checkoutView);
            }
            catch
            {
                ViewBag.Message = "There was an error processing the student information, please try again.";
                return View("Index");
            }

        }

        public ActionResult LongTermView()
        {
            var checkoutView =
            (from checkInCheckout in db.tb_CSUCheckoutCheckin.Where(s => s.CheckinDate == null && s.isLongterm == true)
                join csuStudents in db.tb_CSUStudent on checkInCheckout.CSU_IDFK equals csuStudents.CSU_ID
                select new ViewModels.CkVw()
                {
                    CsuId = checkInCheckout.CSU_IDFK,
                    Ename = csuStudents.ENAME,
                    Name = csuStudents.FIRST_NAME + " " + csuStudents.LAST_NAME,
                    ItemUpc = checkInCheckout.ItemUPCFK,
                    CkOutLabTech = checkInCheckout.CheckoutLabTech,
                    CkOutDt = checkInCheckout.CheckoutDate.Value,
                    LongTerm = checkInCheckout.isLongterm.HasValue,
                    DueDate = checkInCheckout.DueDate.Value,
                    CkOutLoc = checkInCheckout.CheckoutLocationFK,
                    WaitlistHP = checkInCheckout.isWaitlistHP,
                    WaitlistMac = checkInCheckout.isWaitlistMac
                });


            return View("longTermView", checkoutView);
        }

        public ActionResult LongtermCheckout(string csuId, string upc, string upc1, string upc2, DateTime? duedate, bool isWaitlistCheckout = false)
        {
            try
            {
                var view = new ViewModels();
                if (csuId.Length > 9)
                {
                    //Shortens the scanned CSU-ID to 9 characters
                    csuId = csuId.Truncate(9);
                }
                if (view.IsBannedUser(csuId))
                {
                    ViewBag.Message = "That user has been banned from checkout! Or you got this page in error, try again. " + csuId;
                    return View("Index");
                }

                // Checks for Student record
                var csuStudent = GetOrMigrateStudent(csuId);

                //Checks for signed Waiver
                if (WaiverCheck(csuStudent) != true)
                {
                    SessionVariables.waiverCSUId = csuId;
                    SessionVariables.waiverUPC = upc;
                    SessionVariables.waiverUPC1 = upc1;
                    SessionVariables.waiverUPC2 = upc2;
                    SessionVariables.isLongterm = true;
                    SessionVariables.longtermDueDate = duedate;
                    ViewBag.Message = "Records indicate this user has not signed a waiver";
                    return View("Waiver", csuStudent);
                }

                var upcs = new List<string> {upc, upc1, upc2};
                upcs = GetValidUpcs(upc, upc1, upc2);

                //checks for UPC Entry
                if (upcs.Count == 0)
                {
                    ViewBag.Message = "No UPC entered, try again";
                    return View();
                }

                if (upcs.Distinct().Count() != upcs.Count)
                {
                    if (isWaitlistCheckout)
                    {
                        TempData["Message"] = "There are duplicate UPCs, try again.";
                        return RedirectToAction("LongtermWaitlist", "tb_LongtermWaitlist");
                    }
                    ViewBag.Message = "There are duplicate UPCs, try again";
                    return View();
                }

                // for waitlist checkout, check for: 2 upc's max, 1 waitlist laptop max, waitlist laptop is first if multiple upcs
                // this will ensure that if a laptop checkout ends up being invalid, a charger doesn't get processed prior
                if (isWaitlistCheckout)
                {
                    if (upc2 != null)
                    {
                        // only 2 upc's expected for waitlist checkout, so exit
                        TempData["Message"] = "Error. Only certain laptops and their chargers may be checked out via the waitlist.";
                        return RedirectToAction("LongtermWaitlist", "tb_LongtermWaitlist");
                    }
                    // grab item details
                    var itemUpc = db.tb_CSULabInventoryItems.FirstOrDefault(s => s.ItemUPC == upc);
                    var itemUpc1 = db.tb_CSULabInventoryItems.FirstOrDefault(s => s.ItemUPC == upc1);
                    if (upc1 != null)
                    { 
                        if (itemUpc.isWaitlistItem != true && itemUpc1.isWaitlistItem == true)
                        {
                            // swap upc order so waitlist laptop gets processed first
                            upcs = new List<string> { upc1, upc };
                        }
                        else if (itemUpc.isWaitlistItem != true && itemUpc1.isWaitlistItem != true)
                        {
                            // neither item is a waitlist item, so exit
                            TempData["Message"] = "Error. Only certain laptops and their chargers may be checked out via the waitlist.";
                            return RedirectToAction("LongtermWaitlist", "tb_LongtermWaitlist");
                        }
                        else if (itemUpc.isWaitlistItem == true && itemUpc1.isWaitlistItem == true)
                        {
                            // both items are waitlist items, so exit
                            TempData["Message"] = "Error. Only one laptop per user may be checked out via the waitlist.";
                            return RedirectToAction("LongtermWaitlist", "tb_LongtermWaitlist");
                        }
                    }
                    else if (upc1==null && itemUpc.isWaitlistItem!=true)
                    {
                        TempData["Message"] = "Error. Not a valid waitlist item.";
                        return RedirectToAction("LongtermWaitlist", "tb_LongtermWaitlist");
                    }
                }

                // Item Check
                var stuFirst = csuStudent.FIRST_NAME + " " + csuStudent.LAST_NAME;
                var checkoutRecord = new tb_CSUCheckoutCheckin();
                try
                {
                    foreach (var item in upcs)
                    {
                        var itemCheckout = db.tb_CSULabInventoryItems.FirstOrDefault(s => s.ItemUPC == item);
                        var checkedOut = db.tb_CSUCheckoutCheckin.FirstOrDefault(s => s.ItemUPCFK == item && s.CheckinDate == null);

                        if (itemCheckout == null)
                        {
                            ViewBag.Message = "Item #1 " + item + " number not found";
                            return View("Index");
                        }
                        if (checkedOut != null)
                        {
                            ViewBag.Message = "Our Records indicate that item" + item + " is checked-out, please check the item in first! Copy the CSU ID first though so you don't have to ask for the ID back " + csuId;
                            return View("Index");
                        }
                        checkoutRecord.CSU_IDFK = csuId;
                        checkoutRecord.ItemUPCFK = item;
                        checkoutRecord.ItemIDFK = itemCheckout.ItemId;
                        checkoutRecord.CheckoutLabTech = SessionVariables.CurrentUserId;
                        checkoutRecord.CheckoutDate = DateTime.Now;
                        checkoutRecord.CheckoutLocationFK = itemCheckout.ItemLocationFK;
                        checkoutRecord.isLongterm = true;
                        checkoutRecord.DueDate = duedate;

                        // handle waitlist checkout items
                        if (isWaitlistCheckout)
                        {
                            var waitlistStudent = db.tb_LongtermWaitlist.FirstOrDefault(m => m.CSU_ID == csuId);
                            if (itemCheckout.isWaitlistItem == true) // true if item is a waitlist laptop
                            {
                                var isHP = false;
                                var isMac = false;
                                if (itemCheckout.ItemUPC.ToLower().Contains("lt-pro")) { isMac = true; }
                                else if (itemCheckout.ItemUPC.ToLower().Contains("lt-hp")) { isHP = true; }
                                else // item is neither HP nor Mac, so halt checkout
                                {
                                    TempData["Message"] = "Error. This item is identified as a waitlist item, but does not match any HP or Macbook UPCs.";
                                    return RedirectToAction("LongtermWaitlist", "tb_LongtermWaitlist");
                                }

                                /*if (itemCheckout.ReservedTo==null || csuId!=itemCheckout.ReservedTo)
                                {
                                    TempData["Message"] = "UPC " + itemCheckout.ItemUPC + " is not assigned to this " +
                                        "student. User " + csuId + " has been assigned item with UPC " + waitlistStudent.WAITLIST_RESERVED
                                        + ".";
                                    return RedirectToAction("LongtermWaitlist", "tb_LongtermWaitlist");
                                }
                                else if (itemCheckout.ReservedTo==csuId && waitlistStudent.WAITLIST_RESERVED==itemCheckout.ItemUPC) // proceed
                                {
                                    if (isHP) { checkoutRecord.isWaitlistHP = true; }
                                    else if (isMac) { checkoutRecord.isWaitlistMac = true; }

                                    itemCheckout.isCheckedOut = true;
                                }
                                else
                                {
                                    TempData["Message"] = "The checkout could not be completed. The reservation does not match.";
                                    return RedirectToAction("LongtermWaitlist", "tb_LongtermWaitlist");
                                }
                                */
                                if (isHP) { checkoutRecord.isWaitlistHP = true; }
                                else if (isMac) { checkoutRecord.isWaitlistMac = true; }
                                itemCheckout.isCheckedOut = true;
                            }
                        }
                        db.tb_CSUCheckoutCheckin.Add(checkoutRecord);
                        db.SaveChanges();
                        email.CheckoutEmail(csuStudent, checkoutRecord);
                    }
                }
                catch
                {
                    ViewBag.Message = "There was an error during check-out, please try again.";
                    return View();
                }

                ViewBag.Message = stuFirst + ": " + string.Join(" and ", upcs) + " due back on " + duedate.Value.ToString("MM-dd-yyyy") + ".";
                ModelState.Clear();
                if (isWaitlistCheckout)
                {
                    TempData["CheckoutMessage"] = ViewBag.Message;
                    return RedirectToAction("RemoveFromLongtermWaitlist", "tb_LongtermWaitlist", new { csuId = csuId });
                }
                return View("Index");
            }
            catch
            {
                ViewBag.Message = "There was an error processing the student information, please try again.";
                return View();
            }
        }

        // GET: tb_CSUCheckout/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var csuCheckoutCheckin = db.tb_CSUCheckoutCheckin.Find(id);
            if (csuCheckoutCheckin == null)
            {
                return HttpNotFound();
            }
            return View(csuCheckoutCheckin);
        }

        // POST: tb_CSUCheckout/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CheckoutCheckinId,CSU_IDFK,ItemUPCFK,CheckoutLabTech,CheckoutDate,CheckinLabTech,CheckinDate,CheckoutLocationFK,CheckinLocationFK")] tb_CSUCheckoutCheckin tb_CSUCheckoutCheckin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_CSUCheckoutCheckin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_CSUCheckoutCheckin);
        }

        public bool? WaiverCheck(tb_CSUStudent tbs)
        {
            return tbs.SIGNEDWAIVER;
        }
        //alter record for waiver signed
        public ActionResult WaiverSign(string id)
        {
            var v = db.tb_CSUStudent.FirstOrDefault(s => s.CSU_ID == id);
            if (v == null)
            {
                TempData["message"] = "there was an error signing the waiver, please try again";
                return Index();
            }
            v.SIGNEDWAIVER = true;
            email.WaiverEmail(v);
            db.Entry(v).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.Message = "Waiver Recorded, and Checkout Complete!";
            if(SessionVariables.isLongterm == true)
            {
                var dueDateToPass = SessionVariables.longtermDueDate;
                SessionVariables.isLongterm = false;
                SessionVariables.longtermDueDate = null;
                return LongtermCheckout(SessionVariables.waiverCSUId, SessionVariables.waiverUPC, SessionVariables.waiverUPC1, SessionVariables.waiverUPC2, dueDateToPass);
            }
            else
            {
                return Create(SessionVariables.waiverCSUId, SessionVariables.waiverUPC, SessionVariables.waiverUPC1, SessionVariables.waiverUPC2);
            }
        }

        public List<string> GetValidUpcs(params string[] upcs)
        {
            var result = new List<string>();
            result.AddRange(upcs);
            result = result.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            return result;
        }

        // GET: tb_CSUCheckout/Delete/5
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

        // POST: tb_CSUCheckout/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var tb_CSUCheckoutCheckin = db.tb_CSUCheckoutCheckin.Find(id);
            db.tb_CSUCheckoutCheckin.Remove(tb_CSUCheckoutCheckin);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        #region Private Functions

        private tb_CSUStudent GetOrMigrateStudent(string csuId)
        {
            var findStudent = db.tb_CSUStudent.FirstOrDefault(s => s.CSU_ID == csuId);

            if (findStudent != null)
            {
                return findStudent;
            }

            //The student hasn't checked out before, attempt to gather info from CSU Main Database
            try
            {
                var dbHera = new HeraStudents_Entities();
                var noDupesForCheckout =
                    dbHera.v_CSUG_DIRECTORY_ALL_LOCAL_No_Dupes_forCheckinCheckout
                        .FirstOrDefault(s => s.CSU_ID == csuId);

                if (noDupesForCheckout != null)
                {
                    //creates student record if its found
                    var csuStudent = new tb_CSUStudent
                    {
                        CSU_ID = noDupesForCheckout.CSU_ID,
                        EMAIL_ADDRESS = noDupesForCheckout.EMAIL_ADDRESS,
                        ENAME = noDupesForCheckout.ENAME,
                        FIRST_NAME = noDupesForCheckout.FIRST_NAME,
                        LAST_NAME = noDupesForCheckout.LAST_NAME,
                        PHONE = noDupesForCheckout.LAST_NAME,
                        SIGNEDWAIVER = null
                    };
                    db.tb_CSUStudent.Add(csuStudent);
                    db.SaveChanges();
                    findStudent = csuStudent;
                }
                //the student wasn't found in our view of the CSU Directory. What happend?
                email.ErrorEmail(csuId);
                ViewBag.Message = "Student ID # not found in the system, please create a manual entry";
            }
            catch (Exception e)
            {
                string error = e.ToString();
                email.ErrorEmail(error);
            }
            return findStudent;
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



