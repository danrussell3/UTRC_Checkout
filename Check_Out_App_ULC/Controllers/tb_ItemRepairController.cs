using System;
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
                    //repair.Checklist = checklists.First().checkItems;

                    var checklist = checklists.First().checkItems;
                    repair.Checklist = new List<Models.Trello.CheckItemView>();
                    foreach (var checkitem in checklist)
                    {
                        Trello.CheckItemView civ = new Models.Trello.CheckItemView();
                        civ.id = checkitem.id;
                        civ.name = checkitem.name;
                        civ.nameData = checkitem.nameData;
                        civ.pos = checkitem.pos;
                        civ.idChecklist = checkitem.idChecklist;
                        if (checkitem.state == "complete")
                        {
                            civ.state = true;
                        }
                        else
                        {
                            civ.state = false;
                        }
                        repair.Checklist.Add(civ);
                    }

                    repair.RequestDate = checklists.First().name;
                    repair.DueDate = Convert.ToDateTime(card.due).Date;
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
        }

        public ActionResult RequestRepair(string upc, string description, string duedate)
        {
            if (upc == null || description == null)
            {
                TempData["message"] = "To submit a request for repair, you must include both a valid UPC and a description of the issue to be resolved. Please try again.";
                return RedirectToAction("Index");
            }

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
                var user = db.tb_CSULabTechs.FirstOrDefault(m => m.ENAME == SessionVariables.CurrentUserId);
                // submit request by creating a new checklist on the card and adding the description as a comment
                description = description + "  -- Posted by: " + user.First_Name + " " + user.Last_Name + "(" + DateTime.Now.ToString() + ")";
                var commentId = t.PostCardComment(cardId, description);
                var checklistId = t.PostCardChecklist(cardId, DateTime.Now.ToString());

                // convert the string to a DateTimeOffset type
                DateTime newDate = Convert.ToDateTime(duedate).Date;

                string newDueDate = t.PutNewDueDate(cardId, newDate);
                // TO DO: add checklist items


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

        // opens a view to updates an individual repair
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
                    var checklist = checklists.First().checkItems;
                    item.Checklist = new List<Models.Trello.CheckItemView>();
                    foreach (var checkitem in checklist)
                    {
                        Trello.CheckItemView civ = new Models.Trello.CheckItemView();
                        civ.id = checkitem.id;
                        civ.name = checkitem.name;
                        civ.nameData = checkitem.nameData;
                        civ.pos = checkitem.pos;
                        civ.idChecklist = checkitem.idChecklist;
                        if (checkitem.state == "complete")
                        {
                            civ.state = true;
                        }
                        else
                        {
                            civ.state = false;
                        }
                        item.Checklist.Add(civ);
                    }
                    item.RequestDate = checklists.First().name;
                    item.DueDate = card.due;
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

        // updates the due date on an open repair
        public ActionResult UpdateRepairDueDate(string itemUpc, string itemLoc, string newDueDate)
        {
            // convert the string to a DateTimeOffset type
            var newDate = DateTime.Parse(newDueDate).Date;
            //string x = null;

            Trello t = new Models.Trello();
            var cards = t.GetCards(itemLoc);
            string cardId = null;
            foreach (var card in cards)
            {
                if (card.name == itemUpc)
                {
                    cardId = card.id;
                    continue;
                }
            }

            if (cardId != null)
            {
                var date = t.PutNewDueDate(cardId, newDate);

                TempData["message"] = "Updated due date for Item #" + itemUpc + " to " + newDueDate;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "There was an error locating the repair history for Item #" + itemUpc + ". Please try again.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult UpdateChecklist(RepairStatusView repair, string upc)
        {
            Trello t = new Models.Trello();
            var cards = t.GetCards(SessionVariables.CurrentLocation.ToString());
            foreach (var card in cards)
            {
                if (card.name == upc)
                {
                    var checklists = t.GetChecklists(card.id);
                    var checklist = checklists.First().checkItems;
                    // check if checklist item was changed
                    for (var i = 0; i < checklist.Count(); i++)
                    {
                        if ( (repair.Checklist[i].state == true && checklist[i].state == "incomplete") || (repair.Checklist[i].state == false && checklist[i].state == "complete") )
                        {
                            var newState = "incomplete";
                            if (repair.Checklist[i].state == true)
                            {
                                newState = "complete";
                            }
                            var result = t.PutChangeChecklistItem(card.id, checklist[i].id, newState);
                        }
                    }

                    
                }
            }


            return RedirectToAction("Index");
        }

        // Marks any existing checklist items as complete, closes the due date, and marks the repair request as closed
        public ActionResult CloseRepair(string itemUpc, string itemLoc, string description, string confirm)
        {
            if (itemUpc == null || itemLoc == null || description == "" || confirm != "close")
            {
                TempData["message"] = "To close a request for repair, you must include a closing note and confirm that the repair has been resolved. Please try again.";
                return RedirectToAction("Index");
            }
            if (confirm == "nevermind")
            {
                TempData["message"] = "To close a request for repair, you must confirm that the repair has been resolved. Please try again.";
                return RedirectToAction("Index");
            }

            bool success = false;
            Trello t = new Models.Trello();

            // get card id first
            var cards = t.GetCards(itemLoc);
            string cardId = null;
            foreach (var card in cards)
            {
                if (card.name == itemUpc)
                {
                    cardId = card.id;
                    continue;
                }
            }

            if (cardId != null)
            {
                var user = db.tb_CSULabTechs.FirstOrDefault(m => m.ENAME == SessionVariables.CurrentUserId);
                
                // retrieves the checklist and close any open checklist items
                var checklists = t.GetChecklists(cardId);
                var checklistId = checklists.First().id;
                List<Trello.CheckItem> checklistItems = t.GetChecklistItems(checklistId);
                foreach (var item in checklistItems)
                {
                    var result = t.PutChangeChecklistItem(cardId, item.id, "complete");
                }

                // post the close note as a comment
                description = description + "  -- Posted by: " + user.First_Name + " " + user.Last_Name + "(" + DateTime.Now.ToString() + ")";
                var commentId = t.PostCardComment(cardId, description);
                
                // mark the due date as complete
                var dueDateClosed = t.PutCloseDueDate(cardId);

                // determine success
                if (commentId != null && dueDateClosed != null)
                {
                    success = true;
                }
            }

            if (success) // success
            {
                TempData["message"] = "The repair request for Item #" + itemUpc + " has been closed.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "There was a problem closing this repair. Please try again.";
                return RedirectToAction("Index");
            }
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
