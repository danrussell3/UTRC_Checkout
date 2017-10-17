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
    public class tb_ItemRepairController : Controller
    {
        #region Constructors

        private Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();

        #endregion

        #region Public Functions

        // GET: tb_ItemRepair
        public ActionResult Index()
        {
            return View(db.tb_ItemRepair.ToList());
        }

        // GET: tb_ItemRepair/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_ItemRepair tb_ItemRepair = db.tb_ItemRepair.Find(id);
            if (tb_ItemRepair == null)
            {
                return HttpNotFound();
            }
            return View(tb_ItemRepair);
        }

        // GET: tb_ItemRepair/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tb_ItemRepair/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RepairId,AssignedTo,Issue,Notes,DateIssued,DateRepaired,RequestedBy,UnRepairable,RepairedBy,ItemUpcFk,ItemIdFk")] tb_ItemRepair tb_ItemRepair)
        {
            if (ModelState.IsValid)
            {
                db.tb_ItemRepair.Add(tb_ItemRepair);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tb_ItemRepair);
        }

        // GET: tb_ItemRepair/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_ItemRepair tb_ItemRepair = db.tb_ItemRepair.Find(id);
            if (tb_ItemRepair == null)
            {
                return HttpNotFound();
            }
            return View(tb_ItemRepair);
        }

        // POST: tb_ItemRepair/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RepairId,AssignedTo,Issue,Notes,DateIssued,DateRepaired,RequestedBy,UnRepairable,RepairedBy,ItemUpcFk,ItemIdFk")] tb_ItemRepair tb_ItemRepair)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_ItemRepair).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_ItemRepair);
        }

        // GET: tb_ItemRepair/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_ItemRepair tb_ItemRepair = db.tb_ItemRepair.Find(id);
            if (tb_ItemRepair == null)
            {
                return HttpNotFound();
            }
            return View(tb_ItemRepair);
        }

        // POST: tb_ItemRepair/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_ItemRepair tb_ItemRepair = db.tb_ItemRepair.Find(id);
            db.tb_ItemRepair.Remove(tb_ItemRepair);
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
