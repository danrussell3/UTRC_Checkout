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
    public class tb_TimeClockController : Controller
    {
        #region Constructors

        private Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();

        #endregion

        #region Public Functions

        // GET: tb_TimeClock
        public ActionResult Index()
        {
            return View(db.tb_TimeClock.ToList());
        }

        // GET: tb_TimeClock/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_TimeClock = db.tb_TimeClock.Find(id);
            if (tb_TimeClock == null)
            {
                return HttpNotFound();
            }
            return View(tb_TimeClock);
        }

        // GET: tb_TimeClock/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tb_TimeClock/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TimeClockID,UserIDFK,ClockIn,ClockOut,HoursWorked,Edited")] tb_TimeClock tb_TimeClock)
        {
            if (ModelState.IsValid)
            {
                db.tb_TimeClock.Add(tb_TimeClock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tb_TimeClock);
        }

        // GET: tb_TimeClock/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_TimeClock = db.tb_TimeClock.Find(id);
            if (tb_TimeClock == null)
            {
                return HttpNotFound();
            }
            return View(tb_TimeClock);
        }

        // POST: tb_TimeClock/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TimeClockID,UserIDFK,ClockIn,ClockOut,HoursWorked,Edited")] tb_TimeClock tb_TimeClock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_TimeClock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_TimeClock);
        }

        // GET: tb_TimeClock/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_TimeClock = db.tb_TimeClock.Find(id);
            if (tb_TimeClock == null)
            {
                return HttpNotFound();
            }
            return View(tb_TimeClock);
        }

        // POST: tb_TimeClock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var tb_TimeClock = db.tb_TimeClock.Find(id);
            db.tb_TimeClock.Remove(tb_TimeClock);
            db.SaveChanges();
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
