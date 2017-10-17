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
    public class tb_CSULabLocationsController : Controller
    {
        #region Constructors

        private Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();

        #endregion

        #region Public Functions

        // GET: tb_CSULabLocations
        public ActionResult Index()
        {
            return View(db.tb_CSULabLocations.ToList());
        }

        // GET: tb_CSULabLocations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_CSULabLocations = db.tb_CSULabLocations.Find(id);
            if (tb_CSULabLocations == null)
            {
                return HttpNotFound();
            }
            return View(tb_CSULabLocations);
        }

        // GET: tb_CSULabLocations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tb_CSULabLocations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LocationId,LocationNameAcronym,locationName")] tb_CSULabLocations tb_CSULabLocations)
        {
            if (ModelState.IsValid)
            {
                db.tb_CSULabLocations.Add(tb_CSULabLocations);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tb_CSULabLocations);
        }

        // GET: tb_CSULabLocations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_CSULabLocations = db.tb_CSULabLocations.Find(id);
            if (tb_CSULabLocations == null)
            {
                return HttpNotFound();
            }
            return View(tb_CSULabLocations);
        }

        // POST: tb_CSULabLocations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LocationId,LocationNameAcronym,locationName")] tb_CSULabLocations tb_CSULabLocations)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_CSULabLocations).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_CSULabLocations);
        }

        // GET: tb_CSULabLocations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_CSULabLocations = db.tb_CSULabLocations.Find(id);
            if (tb_CSULabLocations == null)
            {
                return HttpNotFound();
            }
            return View(tb_CSULabLocations);
        }

        // POST: tb_CSULabLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var tb_CSULabLocations = db.tb_CSULabLocations.Find(id);
            db.tb_CSULabLocations.Remove(tb_CSULabLocations);
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
