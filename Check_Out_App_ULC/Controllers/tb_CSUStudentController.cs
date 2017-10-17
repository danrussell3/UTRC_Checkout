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
    public class tb_CSUStudentController : SecuredController
    {
        #region Constructors

        Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();
        EmailController email = new EmailController();
        HeraStudents_Entities dbHera = new HeraStudents_Entities();

        #endregion

        #region Public Functions

        // GET: tb_CSUStudent_
        public ActionResult Index()
        {
            return View();
        }


        // GET: tb_CSUStudent_/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // POST: tb_CSUCheckoutCheckin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        //public ActionResult Create()
        //{

        //    return View();
        //}
        //[HttpPost]
        //public ActionResult Create([Bind(Include = "CSU_ID,ENAME,FIRST_NAME,LAST_NAME, EMAIL_ADDRESS,PHONE")] tb_CSUStudent tb_CSUStudent)
        //{
        //    try
        //    {
        //        db.tb_CSUStudent.Add(tb_CSUStudent);
        //        db.SaveChanges();
        //        ViewBag.Message = "Student Successfully Added";
        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        ViewBag.Message = "The Student was NOT added to the system, try again.";
        //        return View("Home");
        //    }
        //}

        // GET: tb_CSUStudent_/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: tb_CSUStudent_/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: tb_CSUStudent_/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: tb_CSUStudent_/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult StudentsLookup(string Firstname, string Lastname)
        {
            //checks for vaild name
            var nameCount =
                dbHera.v_CSUG_DIRECTORY_ALL_LOCAL_No_Dupes_forCheckinCheckout.FirstOrDefault(
                    name => name.LAST_NAME == Lastname);

            //searches by lastname
            if (string.IsNullOrEmpty(Firstname) && nameCount != null)

            {
                ViewBag.Message = "You Searched for " + Lastname;
                object c = dbHera.v_CSUG_DIRECTORY_ALL_LOCAL_No_Dupes_forCheckinCheckout
                    .Where(name => name.LAST_NAME == Lastname).ToList();
                return View("StudentsLookup", c);
            }

            //searches by fullname
            if (!string.IsNullOrEmpty(Firstname) && nameCount != null)
            {
                object l = dbHera.v_CSUG_DIRECTORY_ALL_LOCAL_No_Dupes_forCheckinCheckout
                    .Where(name => name.LAST_NAME == Lastname && name.FIRST_NAME == Firstname).ToList();
                ViewBag.Message = "You Searched for " + Firstname + "" + Lastname;
                return View("StudentsLookup", l);
            }
            else
            {
                ViewBag.Message = "Invalid Search";
                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult EnameLookUp(string ename)
        {
            var enameCount = db.tb_CSUStudent.Count(s => s.ENAME == ename);
            if (enameCount < 1)
            {
                ViewBag.Message = "Invalid Ename";
                return View("Index");
            }
            else
            {
                ViewBag.Message = "You Searched for " + ename;
                object j = db.tb_CSUStudent.Select(e => e.ENAME == ename);
                return View("StudentsLookup", j);
            }
        }

        public ActionResult CsuIdLookUp(string csuId)
        {
            var csuidCount = db.tb_CSUStudent.Count(s => s.CSU_ID == csuId);
            if (csuidCount < 1)
            {
                ViewBag.Message = "Invalid CSU ID #";
                return View("Index");
            }
            else
            {
                ViewBag.Message = "You Searched for " + csuId;
                object o = db.tb_CSUStudent.Where(e => e.CSU_ID == csuId);
                return View("csuIdLookup", o);
            }
        }

        public ActionResult BanUser(string csuID, string reason, bool? isPermBan)
        {
            tb_BannedUserTable banTable = new tb_BannedUserTable();
            var banStu = db.tb_CSUStudent.FirstOrDefault(e => e.CSU_ID == csuID);
            if (banStu == null)
            {
                TempData["Message"] = "No Student record found.";
                return RedirectToAction("BanView");
            }
            banTable.CSU_ID = banStu.CSU_ID;
            banTable.isBanned = true;
            banTable.isPermBanned = isPermBan == true;
            banTable.BanReason = reason;
            banTable.BannedBy = SessionVariables.CurrentUser.ENAME;
            banTable.DateBanned = DateTime.Now;
            db.tb_BannedUserTable.Add(banTable);
            db.SaveChanges();
            //generate an email notice to the banned user
            // now setup the message properties
            email.BanEmail(isPermBan, banStu);
            TempData["Message"] = "User " + csuID + " Banned";
            return RedirectToAction("BanView");
        }

        public ActionResult UnbanUser(string csuID)
        {
            try
            {
                var banTable = new tb_BannedUserTable();
                var banStu = db.tb_BannedUserTable.FirstOrDefault(e => e.CSU_ID == csuID);
                if (banStu == null)
                {
                    TempData["Message"] = "Unable to unban user " + csuID + ".";
                    return View("BanView");
                }
                var usersToUnban = from d in db.tb_BannedUserTable where d.CSU_ID == csuID select d;
                foreach (var u in usersToUnban)
                {
                    if (u.isPermBanned == false)
                    {
                        db.tb_BannedUserTable.Remove(u);
                    }
                }

                // removes all instances of user from tb_BannedUserTable
                db.SaveChanges();
                TempData["Message"] = "User " + csuID + " Unbanned";
                return RedirectToAction("BanView");
            }
            catch
            {
                TempData["Message"] = "The Student" + csuID + " was NOT able to be unbanned, try again.";
                return View("BanView");
            }
            //return null;
        }

        public ActionResult BanView()
        {
            var banViewStu =
            (from bannedUser in db.tb_BannedUserTable.Where(e => e.isBanned == true)
                join stu in db.tb_CSUStudent on bannedUser.CSU_ID equals stu.CSU_ID
                select new ViewModels.BanUserView()
                {
                    CsuId = bannedUser.CSU_ID,
                    Name = stu.FIRST_NAME + " " + stu.LAST_NAME,
                    EName = stu.ENAME,
                    IsBanned = bannedUser.isBanned.Value,
                    IsPermBanned = bannedUser.isPermBanned.Value,
                    DateBanned = bannedUser.DateBanned,
                    BanReason = bannedUser.BanReason,
                    BannedBy = bannedUser.BannedBy,


                });
            ViewBag.Message = TempData["Message"];
            return View("BanView", banViewStu);
        }
        //public ActionResult LoadDB()
        //{
        //    var fooview = new ViewModels();
        //    //fooview.DeleteLocalDB();
        //    fooview.CreateLocalDB();
        //    ViewBag.Message = "Database Updated!";
        //    return View();
        //}

        #endregion

    }
}

