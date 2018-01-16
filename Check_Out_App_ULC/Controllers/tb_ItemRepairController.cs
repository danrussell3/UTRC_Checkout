﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Check_Out_App_ULC.Models;
using static Check_Out_App_ULC.Models.ViewModels;

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
            // get active repairs for current location
            Trello t = new Trello();
            var cards = new List<Trello.Card>();
            cards = t.GetCards(SessionVariables.CurrentLocation.ToString());
            
            // populate a view model by inserting cards
            List<RepairStatusView> view = new List<RepairStatusView>();

            // compile list of cards with active due date, sort by date due
            foreach (var card in cards)
            {
                if (card.due != null && card.dueComplete == false)
                {
                    RepairStatusView repair = new RepairStatusView();
                    repair.ItemUpc = card.name;
                    repair.Comments = t.GetCardComments(card.id);
                    var checklists = t.GetChecklists(card.id);
                    repair.Checklist = checklists.First().checkItems;
                    repair.RequestDate = checklists.First().name;
                    foreach (var b in t.GetBoards())
                    {
                        if (b.id == card.idBoard)
                        {
                            repair.ItemLocation = b.name;
                        }
                    }
                    view.Add(repair);
                }
            }

            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"];
            }

            return View(view);
            //return View(db.tb_ItemRepair.ToList());
        }

        public ActionResult RequestRepair(string upc, string description, string duedate)
        {
            bool success = false;
            Trello t = new Models.Trello();

            // get card id first
            var cards = t.GetCards(SessionVariables.CurrentLocation.ToString());
            string cardId = null;
            foreach (var card in cards)
            {
                if (card.name == upc)
                {
                    cardId = card.id;
                    continue;
                }
            }

            if (cardId != null)
            {
                // submit request by creating a new checklist on the card and adding the description as a comment
                var commentId = t.PostCardComment(cardId, description);
                var checklistId = t.PostCardChecklist(cardId, DateTime.Now.ToString());
                string newDueDate = null;
                newDueDate = t.PutNewDueDate(cardId, duedate);
                // TO DO: add checklist item


                if (commentId != null && checklistId != null && newDueDate != null) // id's were returned, so success
                {
                    success = true;
                }
            }

            if (success) // success
            {
                TempData["message"] = "Item " + upc + " was submitted for repair.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "There was a problem submitting this request. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // updates an open repair
        public ActionResult UpdateRepair(string upc)
        {
            RepairStatusView item = new RepairStatusView();
            Trello t = new Models.Trello();
            var cards = t.GetCards(SessionVariables.CurrentLocation.ToString());
            foreach (var card in cards)
            {
                if (card.name == upc)
                {
                    item.ItemUpc = card.name;
                    item.Comments = t.GetCardComments(card.id);
                    var checklists = t.GetChecklists(card.id);
                    item.Checklist = checklists.First().checkItems;
                    item.RequestDate = checklists.First().name;
                    foreach (var b in t.GetBoards())
                    {
                        if (b.id == card.idBoard)
                        {
                            item.ItemLocation = b.name;
                        }
                    }
                }
            }
            return View(item);
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

        public ActionResult Trello()
        {
            tb_ItemRepair view = new tb_ItemRepair();
            return View("Trello", view);
        }

        //
        public ActionResult GenerateNewCards(string location)
        {
            Trello t = new Trello();
            var result = t.GenerateCards(location);
            return RedirectToAction("Trello");
        }

        // retrieves all boards
        public ActionResult GetBoards()
        {
            Trello t = new Trello();
            var result = t.GetBoards();
            return RedirectToAction("Trello");
        }

        // retrieves all cards
        public ActionResult GetCardsList(string board)
        {
            Trello t = new Trello();
            var result = t.GetCards(board);
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
