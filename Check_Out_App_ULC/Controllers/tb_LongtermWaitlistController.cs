using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Check_Out_App_ULC.Models;
using System.Net;
using System.Data;
using System.Net.Mail;

namespace Check_Out_App_ULC.Controllers
{
    public class tb_LongtermWaitlistController : SecuredController
    {
        #region Constructors

        Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();
        EmailController email = new EmailController();
        HeraStudents_Entities dbHera = new HeraStudents_Entities();

        #endregion

        #region Public Functions

        public ActionResult LongtermWaitlist()
        {
            var waitlist =
            (from waitlistedUsers in db.tb_LongtermWaitlist.Where(e => e.WAITLISTED != null)
             orderby waitlistedUsers.WAITLISTED
             select new ViewModels.LongtermWaitlistView()
             {
                 CsuId = waitlistedUsers.CSU_ID,
                 Waitlisted = waitlistedUsers.WAITLISTED,
                 Name = waitlistedUsers.FIRST_NAME + " " + waitlistedUsers.LAST_NAME,
                 EName = waitlistedUsers.ENAME,
                 WaitlistReason = waitlistedUsers.WAITLIST_REASON,
                 WaitlistType = waitlistedUsers.WAITLIST_TYPE,
                 WaitlistNotified = waitlistedUsers.WAITLIST_NOTIFIED,
                 WaitlistReserved = waitlistedUsers.WAITLIST_RESERVED,
             });

            // grab list of available waitlist UPCs
            var availUpcs = db.tb_CSULabInventoryItems.Where(m => m.isWaitlistItem == true && m.isCheckedOut != true).Select(m => m.ItemUPC).ToList();

            // pre-sort upcs into HP/Mac/Either lists
            var upcsHP = new List<String>();
            var upcsMac = new List<String>();
            
            foreach (var upc in availUpcs)
            {
                var isHP = false;
                var isMac = false;
                if (upc.ToLower().Contains("lt-pro")) { isMac = true; }
                else if (upc.ToLower().Contains("lt-hp")) { isHP = true; }
                if (isHP)
                {
                    upcsHP.Add(upc);
                }
                if (isMac)
                {
                    upcsMac.Add(upc);
                }
            }

            ViewBag.upcsHP = new SelectList(upcsHP);
            ViewBag.upcsMac = new SelectList(upcsMac);
            ViewBag.upcsEither = new SelectList(availUpcs);
            ViewBag.Message = TempData["InventoryMessage"] + " " + TempData["CheckoutMessage"] + " " + TempData["Message"];
            return View("LongtermWaitlist", waitlist);
        }

        public ActionResult LongtermWaitlistCheckout(string csuId, string upc, string upc1)
        {
            var duedate = DateTime.Now.AddDays(14);
            
            return RedirectToAction("LongtermCheckout", "tb_CSUCheckout", new { csuId = csuId, upc = upc, upc1 = upc1, duedate = duedate, isWaitlistCheckout = true });
        }

        public ActionResult AddToLongtermWaitlist(string csuId, string reason, string type)
        {
            var stuToWaitlist = db.tb_CSUStudent.FirstOrDefault(e => e.CSU_ID == csuId);
            var waitlistEntry = db.tb_LongtermWaitlist.FirstOrDefault(e => e.CSU_ID == csuId);
            
            if (stuToWaitlist == null)
            {
                TempData["Message"] = "No Student record found.";
                return RedirectToAction("LongtermWaitlist");
            }
            else if (waitlistEntry != null)
            {
                TempData["Message"] = "User " + csuId + " has already been added to the waitlist at " + waitlistEntry.WAITLISTED;
                return RedirectToAction("LongtermWaitlist");
            }
            else
            {
                waitlistEntry = new tb_LongtermWaitlist();
                waitlistEntry.WAITLISTED = DateTime.Now;
                waitlistEntry.WAITLIST_REASON = reason;
                waitlistEntry.WAITLIST_TYPE = type;
                waitlistEntry.WAITLIST_NOTIFIED = null;
                waitlistEntry.WAITLIST_RESERVED = null;
                waitlistEntry.CSU_ID = stuToWaitlist.CSU_ID;
                waitlistEntry.ENAME = stuToWaitlist.ENAME;
                waitlistEntry.FIRST_NAME = stuToWaitlist.FIRST_NAME;
                waitlistEntry.LAST_NAME = stuToWaitlist.LAST_NAME;
                db.tb_LongtermWaitlist.Add(waitlistEntry);
                db.SaveChanges();

                // generate an email notice to the user that they've been added to the waitlist
                //email.WaitlistAddEmail(stuToWaitlist);

                TempData["Message"] = "User " + csuId + " added to the waitlist";
                return RedirectToAction("WaitlistStatusCheck");
            }
        }

        public ActionResult RemoveFromLongtermWaitlist(string csuId)
        {
            var student = db.tb_CSUStudent.FirstOrDefault(e => e.CSU_ID == csuId);
            var stuToRemove = db.tb_LongtermWaitlist.FirstOrDefault(e => e.CSU_ID == csuId);
            if (student == null)
            {
                TempData["Message"] = "No Student record found.";
                return RedirectToAction("LongtermWaitlist");
            }
            else if (stuToRemove.WAITLISTED == null)
            {
                TempData["Message"] = "User " + csuId + " has already been removed from the waitlist.";
                return RedirectToAction("LongtermWaitlist");
            }
            else
            {
                // if HasReserved is set, null it and set associated ReservedTo to null also
                if (stuToRemove.WAITLIST_RESERVED != null)
                {
                    var itemupc = stuToRemove.WAITLIST_RESERVED;
                    var item = db.tb_CSULabInventoryItems.FirstOrDefault(s => s.ItemUPC == itemupc);
                    item.ReservedTo = null;
                }

                db.tb_LongtermWaitlist.Remove(stuToRemove);
                db.SaveChanges();
                
                TempData["Message"] = "User " + csuId + " removed from the waitlist.";
                return RedirectToAction("WaitlistStatusCheck");
            }
        }

        // checks waitlist against available laptops, makes reservations/notifications as needed
        // triggered by any waitlist add, waitlist remove, checkin of waitlist item, or waitlist view
        public ActionResult WaitlistStatusCheck()
        {
            // grab current waitlist
            var waitlist = db.tb_LongtermWaitlist.Where(m => m.WAITLISTED != null).OrderBy(m => m.WAITLISTED).ToList();

            // check for reservations more than 50 hours old
            foreach (var student in waitlist)
            {
                if (student.WAITLIST_NOTIFIED != null)
                {
                    DateTime expiration = (DateTime)student.WAITLIST_NOTIFIED;
                    expiration = expiration.AddDays(2);
                    expiration = expiration.AddHours(2);
                    if (DateTime.Now > expiration)
                    {
                        // reservation has expired, so remove from waitlist and notify student
                        // if WAITLIST_RESERVED is set, null it and set associated ReservedTo to null also
                        if (student.WAITLIST_RESERVED != null)
                        {
                            var itemupc = student.WAITLIST_RESERVED;
                            var item = db.tb_CSULabInventoryItems.FirstOrDefault(s => s.ItemUPC == itemupc);
                            item.ReservedTo = null;
                        }
                        var stuDetails = db.tb_CSUStudent.FirstOrDefault(e => e.CSU_ID == student.CSU_ID);
                        //email.WaitlistRemoveAfter48HoursEmail(stuDetails);
                        db.tb_LongtermWaitlist.Remove(student);
                        db.SaveChanges();
                    }
                }
            }

            // get updated waitlist
            waitlist = db.tb_LongtermWaitlist.Where(m => m.WAITLISTED != null).OrderBy(m => m.WAITLISTED).ToList();

            
            // CAN LIKELY REMOVE THIS CHECK, or change it to double-check that a notification wasn't made in error
            // check in case a student had a reservation but the item that was subsequently taken out of circulation
            // if so, remove reserved status so the student goes back to the top of the waitlist
            foreach (var student in waitlist)
            {        
                if (student.WAITLIST_RESERVED != null)
                {
                    var reservedItem = db.tb_CSULabInventoryItems.FirstOrDefault(e => e.ItemUPC == student.WAITLIST_RESERVED);
                    if (reservedItem.ReservedTo!=student.CSU_ID)
                    {
                        reservedItem.ReservedTo = null;
                        student.WAITLIST_RESERVED = null;
                        student.WAITLIST_NOTIFIED = null;
                        var stuDetails = db.tb_CSUStudent.FirstOrDefault(e => e.CSU_ID == student.CSU_ID);
                        //email.WaitlistReservationProblemEmail(stuDetails, reservedItem);
                    }
                    db.SaveChanges();
                }
            }

            var available = db.tb_CSULabInventoryItems.Where(m => (m.isWaitlistItem == true) && m.isCheckedOut != true && m.ReservedTo == null).ToList();
            var numAvailableHPs = 0;
            var numAvailableMacs = 0;

            // tally number of available HPs and Macs
            foreach (var item in available)
            {
                if (item.ItemUPC.ToLower().Contains("lt-pro")) { numAvailableMacs++; }
                else if (item.ItemUPC.ToLower().Contains("lt-hp")) { numAvailableHPs++; }
            }

            // iterate through list of available items, notify students as needed that an item is available
            foreach (var item in available)
            {
                var isHP = false;
                var isMac = false;
                if (item.ItemUPC.ToLower().Contains("lt-pro")) { isMac = true; }
                else if (item.ItemUPC.ToLower().Contains("lt-hp")) { isHP = true; }
                else
                {
                    TempData["InventoryMessage"] = "Item " + item.ItemUPC + " doesn't match recognized UPC codes. UPC for HP's" +
                        "should contain 'LT-HP', and UPC for Macbook Pro's should contain 'LT-PRO'.";
                    return RedirectToAction("LongtermWaitlist");
                }

                // TO DO
                // notify students of availability based on tally of available laptops
                foreach (var student in waitlist)
                {
                    if ((student.WAITLIST_NOTIFIED==null && student.WAITLIST_RESERVED==null) &&
                        (student.WAITLIST_TYPE=="Either" || 
                        (student.WAITLIST_TYPE=="HP" && isHP==true) ||
                        (student.WAITLIST_TYPE=="Macbook" && isMac==true)))
                    {
                        /*
                        // match between student need and available item, so reserve item to student and student to item
                        var itemToReserve = db.tb_CSULabInventoryItems.FirstOrDefault(e => e.ItemUPC == item.ItemUPC);
                        itemToReserve.ReservedTo = student.CSU_ID;
                        item.ReservedTo = student.CSU_ID; // so next student can't also be assigned to item
                        */
                        
                        // now notify student, record notification time
                        var stuToNotify = db.tb_LongtermWaitlist.FirstOrDefault(e => e.CSU_ID == student.CSU_ID);
                        var stuDetails = db.tb_CSUStudent.FirstOrDefault(e => e.CSU_ID == student.CSU_ID);
                        //email.WaitlistItemReadyEmail(stuDetails);
                        stuToNotify.WAITLIST_NOTIFIED = DateTime.Now;
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("LongtermWaitlist");
        }

        #endregion

    }
}

