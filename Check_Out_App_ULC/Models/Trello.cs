using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;
using Check_Out_App_ULC.Models;
using Check_Out_App_ULC.Controllers.Api;
using RestSharp;

namespace Check_Out_App_ULC.Models
{
    public class Trello
    {
        private readonly Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();
        private RestClient client;
        private const string apiKey = "96ea6537d9e333b29feda8dcfe38f48d";
        private const string apiToken = "bc91a392cd8b86d9a0e26fc68627ea150ffaa6791598d55ba252680576fd42d3";
        private const string sharedUserId = "utrccheckout";

        public Trello()
        {
            client = new RestClient("https://api.trello.com/1/");
        }

        public class Board
        {
            public string id { get; set; }
            public string name { get; set; }
            public string desc { get; set; }
            public string descData { get; set; }
            public bool closed { get; set; }
            public string idOrganization { get; set; }
            public bool pinned { get; set; }
            public string url { get; set; }
            public string shortUrl { get; set; }
            public Prefs prefs { get; set; }
            public LabelNames labelNames { get; set; }

            public class Prefs
            {
                public string permissionLevel { get; set; }
                public string voting { get; set; }
                public string comments { get; set; }
                public string invitations { get; set; }
                public bool selfJoin { get; set; }
                public bool cardCovers { get; set; }
                public string cardAging { get; set; }
                public bool calendarFeedEnabled { get; set; }
                public string background { get; set; }
                public string backgroundImage { get; set; }
                public List<BackgroundImage> backgroundImageScaled { get; set; }
                public bool backgroundTile { get; set; }
                public string backgroundBrightness { get; set; }
                public bool canBePublic { get; set; }
                public bool canBeOrg { get; set; }
                public bool canBePrivate { get; set; }
                public bool canInvite { get; set; }
            }

            public class BackgroundImage
            {
                public string width { get; set; }
                public string height { get; set; }
                public string url { get; set; }
            }

            public class LabelNames
            {
                public string green { get; set; }
                public string yellow { get; set; }
                public string orange { get; set; }
                public string red { get; set; }
                public string purple { get; set; }
                public string blue { get; set; }
                public string sky { get; set; }
                public string lime { get; set; }
                public string pink { get; set; }
                public string black { get; set; }
            }
        }

        public class Card
        {
            public string id { get; set; }
            public Badges badges { get; set; }
            public List<string> checkItemStates { get; set; }
            public bool closed { get; set; }
            public string dateLastActivity { get; set; }
            public string desc { get; set; }
            public DescData descData { get; set; }
            public DateTime? due { get; set; }
            public bool? dueComplete { get; set; }
            public string email { get; set; }
            public string idAttachmentCover { get; set; }
            public string idBoard { get; set; }
            public List<string> idChecklists { get; set; }
            public List<string> idLabels { get; set; }
            public string idList { get; set; }
            public List<string> idMembers { get; set; }
            public List<string> idMembersVoted { get; set; }
            public int idShort { get; set; }
            public List<Label> labels { get; set; }
            public bool manualCoverAttachment { get; set; }
            public string name { get; set; }
            public float pos { get; set; }
            public string shortLink { get; set; }
            public string shortUrl { get; set; }
            public bool subscribed { get; set; }
            public string url { get; set; }

            public class DescData
            {
                public Emoji emoji { get; set; }
            }

            public class Emoji
            {
                public string morty { get; set; }
            }

            public class Label
            {
                public string id { get; set; }
                public string idBoard { get; set; }
                public string name { get; set; }
                public string color { get; set; }
                public int uses { get; set; }
            }

            public class Badges
            {
                public int votes { get; set; }
                public bool viewingMemberVoted { get; set; }
                public bool subscribed { get; set; }
                public string fogbugz { get; set; }
                public int checkItems { get; set; }
                public int checkItemsChecked { get; set; }
                public int comments { get; set; }
                public int attachments { get; set; }
                public bool description { get; set; }
                public string due { get; set; }
                public bool dueComplete { get; set; }
            }
        }

        public class Comment
        {
            public string text { get; set; }
            public string date { get; set; }
            public string commentId { get; set; }
            public string cardId { get; set; }
        }

        public class Checklist
        {
            public string id { get; set; }
            public string name { get; set; }
            public string idBoard { get; set; }
            public string idCard { get; set; }
            public int pos { get; set; }
            public List<CheckItem> checkItems { get; set; }

        }

        public class CheckItem
        {
            public string state { get; set; }
            public string idChecklist { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string nameData { get; set; }
            public int pos { get; set; }
        }

        public class CheckItemView
        {
            public bool state { get; set; }
            public string idChecklist { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string nameData { get; set; }
            public int pos { get; set; }
            public bool? delete { get; set; }
        }

        public List<Board> GetBoards()
        {
            string apiUrl = "members/" + sharedUserId + "/boards";
            var request = new RestRequest(apiUrl, Method.GET);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);

            // execute the request
            var response = client.Execute<List<Board>>(request);
            List<Board> boards = response.Data;
            
            return boards;
        }

        public List<Card> GetCards(string boardName)
        {
            var boards = GetBoards();
            List<string> boardIds = new List<string>();
            foreach (var b in boards)
            {
                if(b.name == boardName || boardName == "notset")
                {
                    boardIds.Add(b.id);
                }
            }

            List<Card> cardList = new List<Card>();
            foreach (var boardId in boardIds)
            {
                string apiUrl = "boards/" + boardId + "/cards";
                var request = new RestRequest(apiUrl, Method.GET);

                // add required parameters
                request.AddParameter("key", apiKey);
                request.AddParameter("token", apiToken);

                // execute the request
                var response = client.Execute<List<Card>>(request);
                List<Card> cards = response.Data;
                foreach (var c in cards)
                {
                    if (c.due != null)
                    {
                        c.due = Convert.ToDateTime(c.due).AddHours(12);
                        c.due = Convert.ToDateTime(c.due).Date;
                    }
                    cardList.Add(c);
                }
            }

            // if location is all, sort the cards to show most recent first
            if (boardName == "notset")
            {
                // TO DO

            }

            return cardList;
        }

        // retrieves the id of the list for the given board name and list name
        public string GetListId(string boardName, string listName)
        {
            var boards = GetBoards();
            string boardId = null;
            foreach (var b in boards)
            {
                if (b.name == boardName)
                {
                    boardId = b.id;
                }
            }

            if (boardId != null)
            {
                string apiUrl = "boards/" + boardId + "/lists";
                var request = new RestRequest(apiUrl, Method.GET);

                // add required parameters
                request.AddParameter("key", apiKey);
                request.AddParameter("token", apiToken);

                // execute the request
                var response = client.Execute(request);

                // parse the response
                var checklists = JArray.Parse(response.Content);
                string itemsListId = null;
                foreach (var c in checklists)
                {
                    if (c["name"].ToString() == listName)
                    {
                        itemsListId = c["id"].ToString();
                    }
                }
                return itemsListId;
            }
            else
            {
                return null;
            }
        }

        // get checklists for a card
        public List<Checklist> GetChecklists(string cardId)
        {
            string apiUrl = "cards/" + cardId + "/checklists";
            var request = new RestRequest(apiUrl, Method.GET);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);

            // execute the request
            var response = client.Execute<List<Checklist>>(request);
            
            return response.Data;
        }

        // retrieves a list of CheckItems from the checklist matching the provided id
        public List<CheckItem> GetChecklistItems(string checklistId)
        {
            string apiUrl = "checklists/" + checklistId + "/checkItems";
            var request = new RestRequest(apiUrl, Method.GET);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);

            // execute the request
            var response = client.Execute<List<CheckItem>>(request);

            return response.Data;
        }

        // retrieve comments from a card
        public List<Comment> GetCardComments(string cardId, string since = null)
        {
            string apiUrl = "cards/" + cardId + "/actions";
            var request = new RestRequest(apiUrl, Method.GET);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);

            // add optional parameters
            if (since != null) { request.AddParameter("since", since); }

            // execute the request
            var response = client.Execute<List<Card>>(request);
            var data = JArray.Parse(response.Content);
            List<Comment> comments = new List<Comment>();
            foreach (var item in data)
            {
                Comment c = new Comment();
                c.text = item["data"]["text"].ToString();
                c.date = item["date"].ToString();
                c.commentId = item["id"].ToString();
                c.cardId = item["data"]["card"]["id"].ToString();
                comments.Add(c);
            }
            return comments;
        }

        // sets new due date for card (due date of repair)
        public string PutNewDueDate(string id, DateTime duedate)
        {
            /*
            if (duedate == null)
            {
                // set default due date
                duedate = DateTime.Now.AddDays(14).ToString();
            }
            */

            string apiUrl = "cards/" + id;
            var request = new RestRequest(apiUrl, Method.PUT);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);
            request.AddParameter("due", duedate.Date);

            // execute the request
            var response = client.Execute(request);
            var cardDetails = JObject.Parse(response.Content);
            //var newDate = cardDetails["due"];
            var newDate = Convert.ToDateTime(cardDetails["due"]).Date;
            //var newDate3 = newDate2.Date;

            return newDate.ToString();
        }

        // sets due date for card as closed (due date of repair)
        public string PutCloseDueDate(string id)
        {
            string apiUrl = "cards/" + id;
            var request = new RestRequest(apiUrl, Method.PUT);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);
            request.AddParameter("dueComplete", "true");

            // execute the request
            var response = client.Execute(request);
            var cardDetails = JObject.Parse(response.Content);
            var completeStatus = cardDetails["dueComplete"].ToString();

            return completeStatus;
        }

        // sets due date for card as open (due date of repair)
        public string PutOpenDueDate(string id)
        {
            string apiUrl = "cards/" + id;
            var request = new RestRequest(apiUrl, Method.PUT);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);
            request.AddParameter("dueComplete", "false");

            // execute the request
            var response = client.Execute(request);
            var cardDetails = JObject.Parse(response.Content);
            var completeStatus = cardDetails["dueComplete"].ToString();

            return completeStatus;
        }

        // marks a checklist item as complete
        public string PutChangeChecklistItem(string cardId, string checkitemId, string state)
        {
            string apiUrl = "cards/" + cardId + "/checkItem/" + checkitemId;
            var request = new RestRequest(apiUrl, Method.PUT);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);
            request.AddParameter("state", state);

            // execute the request
            var response = client.Execute(request);
            var itemDetails = JObject.Parse(response.Content);
            var itemState = itemDetails["state"].ToString();

            return itemState;
        }

        // deletes a checklist item
        public void DeleteChecklistItem(string cardId, string checkitemId)
        {
            string apiUrl = "cards/" + cardId + "/checkItem/" + checkitemId;
            var request = new RestRequest(apiUrl, Method.DELETE);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);

            // execute the request
            var response = client.Execute(request);
            var itemDetails = JObject.Parse(response.Content);
        }

        // deletes a checklist
        public string DeleteChecklist(string cardId, string checklistId)
        {
            string apiUrl = "cards/" + cardId + "/checklists/" + checklistId;
            var request = new RestRequest(apiUrl, Method.DELETE);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);

            // execute the request
            var response = client.Execute(request);
            var itemDetails = JObject.Parse(response.Content);
            var itemState = itemDetails["state"].ToString();

            return itemState;
        }

        public string PostBoard(string name, string desc = null, bool defaultLists = false)
        {
            string apiUrl = "boards/";
            var request = new RestRequest(apiUrl, Method.POST);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);
            request.AddParameter("name", name);
            if (desc != null) { request.AddParameter("desc", desc); }
            request.AddParameter("defaultLists", defaultLists);

            // execute the request
            var response = client.Execute(request);
            var jarray = JArray.Parse(response.Content);
            var newBoardId = jarray["id"].ToString();

            return newBoardId;
        }

        public string PostBoardChecklist(string id, string name = null)
        {
            string apiUrl = "boards/" + id + "/checklists";
            var request = new RestRequest(apiUrl, Method.POST);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);
            request.AddParameter("name", name);

            // execute the request
            var response = client.Execute(request);
            var jarray = JArray.Parse(response.Content);
            var newChecklistId = jarray["id"].ToString();

            return newChecklistId;
        }

        public string PostCard(string idList, string name, string desc = null, string pos = null, string date = null, string idLabels = null)
        {
            string apiUrl = "cards";
            var request = new RestRequest(apiUrl, Method.POST);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);
            request.AddParameter("idList", idList);
            request.AddParameter("name", name);
            if (desc != null) { request.AddParameter("desc", desc); }
            if (pos != null) { request.AddParameter("pos", pos); }
            if (date != null) { request.AddParameter("date", date); }
            if (idLabels != null) { request.AddParameter("idLabels", idLabels); }

            // execute the request
            var response = client.Execute(request);
            var jarray = JObject.Parse(response.Content);
            var newCardId = jarray["id"].ToString();

            return newCardId;
        }

        public string PostCardComment(string id, string text)
        {
            string apiUrl = "cards/" + id + "/actions/comments";
            var request = new RestRequest(apiUrl, Method.POST);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);
            request.AddParameter("text", text);
            
            // execute the request
            var response = client.Execute(request);
            var jarray = JObject.Parse(response.Content);
            var newCommentId = jarray["id"].ToString();
            //var newCommentId = response.Content

            return newCommentId;
        }

        public string PostCardChecklist(string id, string name = null, string idChecklistSource = null)
        {
            string apiUrl = "cards/" + id + "/checklists";
            var request = new RestRequest(apiUrl, Method.POST);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);
            request.AddParameter("name", name);
            if (idChecklistSource != null) { request.AddParameter("idChecklistSource", idChecklistSource); }

            // execute the request
            var response = client.Execute(request);
            var jarray = JObject.Parse(response.Content);
            var newChecklistId = jarray["id"].ToString();

            return newChecklistId;
        }

        public string PostCardChecklistItem(string checklistId, string name)
        {
            string apiUrl = "checklists/" + checklistId + "/checkItems";
            var request = new RestRequest(apiUrl, Method.POST);

            // add required parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("token", apiToken);
            request.AddParameter("name", name);

            // execute the request
            var response = client.Execute(request);
            var jobject = JObject.Parse(response.Content);
            var newChecklistItemId = jobject["id"].ToString();

            return newChecklistItemId;
        }

        // for given location, this method checks the existing cards against the location's UPCs;
        // if a UPC doesn't already have a card, it creates one
        // returns the number of new cards created
        public int GenerateCards(string location)
        {
            // get boardId for location
            var boards = GetBoards();
            string boardId = null;
            foreach (var b in boards)
            {
                if(b.name == location)
                {
                    boardId = b.id;
                }
            }
            var listId_Items = GetListId(location, "Items");

            // get list of upcs in inventory
            var upcs = db.tb_CSULabInventoryItems.Where(m => m.ItemLocationFK == location);

            // get list of existing cards on board
            var cards = GetCards(location);

            // if upc doesn't have a card, create one
            var numCardsCreated = 0;

            foreach (var upc in upcs)
            {
                // for each upc in inventory, see if a card already exists
                bool alreadyExists = false;
                foreach (var card in cards)
                {
                    if (card.name == upc.ItemUPC)
                    {
                        alreadyExists = true;
                        continue;
                    }
                }

                if (alreadyExists == false)
                {
                    // create new card for that upc
                    PostCard(listId_Items, upc.ItemUPC, desc: upc.ItemDescription);
                    numCardsCreated++;
                }
            }
            // return number of cards created
            return numCardsCreated;
        }
    }
}