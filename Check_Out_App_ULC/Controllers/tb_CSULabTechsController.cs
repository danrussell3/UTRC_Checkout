using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Check_Out_App_ULC.Models;

namespace Check_Out_App_ULC.Controllers
{
    public class tb_CSULabTechsController : SecuredController
    {
        #region Constructors

        private readonly HeraStudents_Entities dbHera = new HeraStudents_Entities();
        private Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();
        ViewModels.StudentsLookup Model = new ViewModels.StudentsLookup();
        private readonly tb_CSULabTechs newTech = new tb_CSULabTechs();

        #endregion

        #region Public Functions

        public ActionResult newLabTech(string ename, string loc)
        {
            try
            {
                var labename =
                    dbHera.v_CSUG_DIRECTORY_ALL_LOCAL_No_Dupes_forCheckinCheckout.FirstOrDefault(s => s.ENAME == ename);

                if (string.IsNullOrEmpty(ename) || labename == null)
                {
                    ViewBag.Message = "Invalid Ename";
                    return View("Index");
                }

                newTech.CSU_ID = labename.CSU_ID;
                newTech.ENAME = labename.ENAME;
                newTech.EMAIL = labename.EMAIL_ADDRESS;
                newTech.First_Name = labename.FIRST_NAME;
                newTech.Last_Name = labename.LAST_NAME;
                newTech.PHONE = labename.PHONE;
                newTech.UserRights = true;
                newTech.Active = true;
                newTech.EnabledBy = SessionVariables.CurrentUserId;
                newTech.EnabledDateTime = DateTime.Now;
                newTech.ManagerRights = null;
                newTech.DisabledDateTime = null;
                newTech.LocationId = loc;
                db.tb_CSULabTechs.Add(newTech);
                db.SaveChanges();
                ViewBag.Message = newTech.First_Name + " " + newTech.Last_Name + " Successfully Added!";

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "There was an Error adding that Tech, please try again.";
                return View("Index");
            }
        }

        public ActionResult Manager(string id)
        {
            try
            {
                var labename = db.tb_CSULabTechs.FirstOrDefault(s => s.ENAME == id);
                if (labename.ManagerRights == true)
                {
                    ViewBag.Message = "That user is already a manager";
                    return View("Index");
                }
                labename.ManagerRights = true;
                db.Entry(labename).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = labename + " is now a manager";
                var viewBuilder = db.tb_CSULabTechs.OrderBy(s => s.UserID);
                return View("Index", viewBuilder);
            }
            catch
            {
                ViewBag.Message = "There was an error finding the manager, try again";
                return View("Index");
            }
        }

        // GET: tb_CSULabTechs
        public ActionResult Index()
        {
            ViewBag.Message = TempData["message"];
            return View(db.tb_CSULabTechs.Where(s => s.UserRights == true).ToList());
        }

        public ActionResult TestSling()
        {
            ViewBag.Message = TempData["Message"];
            return View("TestSling");
        }

        // GET: tb_CSULabTechs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_CSULabTechs = db.tb_CSULabTechs.Find(id);
            if (tb_CSULabTechs == null)
            {
                return HttpNotFound();
            }
            return View(tb_CSULabTechs);
        }

        // GET: tb_CSULabTechs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_CSULabTechs = db.tb_CSULabTechs.Find(id);
            if (tb_CSULabTechs == null)
            {
                return HttpNotFound();
            }
            return View(tb_CSULabTechs);
        }

        // POST: tb_CSULabTechs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
        [Bind(
            Include = "UserID,LocationId,First_Name,Last_Name,CSU_ID,EMAIL,PHONE,ENAME,UserRights,DisabledDateTime")] tb_CSULabTechs tb_CSULabTechs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_CSULabTechs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_CSULabTechs);
        }

        // GET: tb_CSULabTechs/Delete/5

        //[HttpPost]
        public ActionResult Disable(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var disable = db.tb_CSULabTechs.FirstOrDefault(m => m.ENAME == id);
            if (disable == null)
            {
                return HttpNotFound();
            }
  
            disable.ManagerRights = null;
            disable.UserRights = false;
            disable.DisabledDateTime = DateTime.Now;
            disable.DisabledBy = SessionVariables.CurrentUserId;
            disable.Active = false;
            db.Entry(disable).State = EntityState.Modified;
            db.SaveChanges();
            var name = disable.First_Name + " " + disable.Last_Name;
            ViewBag.Message = "User " + name + " disabled.";
            TempData["message"] = ViewBag.Message;
            return RedirectToAction("Index");

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
