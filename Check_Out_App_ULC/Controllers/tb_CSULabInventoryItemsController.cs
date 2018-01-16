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
    public class tb_CSULabInventoryItemsController : SecuredController
    {
        #region Constructors

        private Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();


        #endregion

        #region Public Functions

        public ActionResult Index()
        {
            return View(db.tb_CSULabInventoryItems.ToList().OrderByDescending(s => s.ItemUPC));
        }

        public ActionResult SearchUpc(string id)
        {
            var x = db.tb_CSULabInventoryItems.Where(s => s.ItemUPC == id);
            if (!String.IsNullOrEmpty(id))
            {
                return View("Index", x.ToList());
            }
            ViewBag.Message = "Invalid Item #";
            return View();
        }

        // GET: tb_CSULabInventoryItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_CSULabInventoryItems = db.tb_CSULabInventoryItems.Find(id);
            if (tb_CSULabInventoryItems == null)
            {
                return HttpNotFound();
            }
            return View(tb_CSULabInventoryItems);
        }

        // GET: tb_CSULabInventoryItems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tb_CSULabInventoryItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemId,ItemUPC,ItemDescription,ItemSerialNumber,ItemLocationFK,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn,isWaitlistItem")] tb_CSULabInventoryItems tb_CSULabInventoryItems)
        {
            if (ModelState.IsValid)
            {
                tb_CSULabInventoryItems.ItemSerialNumber = tb_CSULabInventoryItems.ItemSerialNumber.ToUpper();
                tb_CSULabInventoryItems.ItemUPC = tb_CSULabInventoryItems.ItemUPC.ToUpper();
                tb_CSULabInventoryItems.CreatedOn = DateTime.Now;
                tb_CSULabInventoryItems.CreatedBy = SessionVariables.CurrentUserId;
                db.tb_CSULabInventoryItems.Add(tb_CSULabInventoryItems);
                var itemLocation = tb_CSULabInventoryItems.ItemLocationFK;
                db.SaveChanges();

                // run Trello UPC check to add a repair card for the new item
                Trello t = new Trello();
                t.GenerateCards(itemLocation);

                ViewBag.Message = "Item Added to Inventory";
                return RedirectToAction("Index");
            }
            return View(tb_CSULabInventoryItems);
        }

        // GET: tb_CSULabInventoryItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_CSULabInventoryItems = db.tb_CSULabInventoryItems.Find(id);
            if (tb_CSULabInventoryItems == null)
            {
                return HttpNotFound();
            }
            return View(tb_CSULabInventoryItems);
        }

        // POST: tb_CSULabInventoryItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemId,ItemUPC,ItemDescription,ItemSerialNumber,ItemLocationFK,isWaitlistItem")] tb_CSULabInventoryItems tb_CSULabInventoryItems)
        {
            if (ModelState.IsValid)
            {
                tb_CSULabInventoryItems.ItemSerialNumber = tb_CSULabInventoryItems.ItemSerialNumber.ToUpper();
                tb_CSULabInventoryItems.ItemUPC = tb_CSULabInventoryItems.ItemUPC.ToUpper();
                tb_CSULabInventoryItems.UpdatedOn = DateTime.Now;
                tb_CSULabInventoryItems.UpdatedBy = SessionVariables.CurrentUserId;
                db.Entry(tb_CSULabInventoryItems).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_CSULabInventoryItems);
        }

        // GET: tb_CSULabInventoryItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tb_CSULabInventoryItems = db.tb_CSULabInventoryItems.Find(id);
            if (tb_CSULabInventoryItems == null)
            {
                return HttpNotFound();
            }
            return View(tb_CSULabInventoryItems);
        }

        // POST: tb_CSULabInventoryItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var tb_CSULabInventoryItems = db.tb_CSULabInventoryItems.Find(id);
            db.tb_CSULabInventoryItems.Remove(tb_CSULabInventoryItems);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult IsOut(string ItemUPCFK)
        {
            var isChecked = new tb_CSULabInventoryItems();
            isChecked.isCheckedOut = true;
            return View();
        }

        //show a page with the Mac Update instructions.
        public ActionResult upInstruc(string id)
        {
            return View("upInstruc");
        }

        public ActionResult WaitlistItems()
        {
            return View(db.tb_CSULabInventoryItems.Where(m => m.isWaitlistItem==true).ToList().OrderByDescending(s => s.ItemUPC));
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
