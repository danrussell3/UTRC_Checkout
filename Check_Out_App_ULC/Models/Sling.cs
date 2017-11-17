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

namespace Check_Out_App_ULC.Models
{

    public class Sling
    {
        public static readonly string apiKey = "e98f0ab7fac452dded22f6c01d493e73";

        public class AccountLoginObject
        {
            public string email;
            public string password;
            public string snsPlatform;
            public string snsToken;
        }

        public class SlingUser
        {
            public string status { get; set; }
            public string name { get; set; }
        }

        /*
        public class SlingArticleComments
        {
            public string commentContent { get; set; }
            public string commenterName { get; set; }
            public DateTime commentTime { get; set; }
        }
        */
        
        public static SlingUser Login(string e, string pw, string snsP, string snsT)
        {
            string apiUrl = "https://api.sling.is/v1/account/login",
                resultString = "";

            AccountLoginObject postObject = new AccountLoginObject()
            {
                email = String.IsNullOrEmpty(e) ? "json" : e,
                password = pw,
                snsPlatform = snsP,
                snsToken = snsT
            };

            string debug = JsonConvert.SerializeObject(postObject);
            //Stringify the data and convert to byte
            byte[] postData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(postObject));
            
            HttpWebRequest request = WebRequest.Create(apiUrl) as HttpWebRequest;
            request.Method = "POST";
            request.Timeout = 60000;
            request.KeepAlive = false;
            request.ContentLength = postData.Length;
            request.ContentType = "application/json";
            request.Headers.Add("X-API-TOKEN", apiKey);
            request.CookieContainer = new CookieContainer();

            using (Stream stream = request.GetRequestStream())
                stream.Write(postData, 0, postData.Length);

            using (WebResponse webResponse = request.GetResponse())
                using (Stream stream = webResponse.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        resultString = reader.ReadToEnd();

            if (!String.IsNullOrEmpty(resultString))
            {
                var result = JObject.Parse(resultString);
                var user = result["user"].ToObject<SlingUser>();

                string status = result["user"]["status"].ToString();

                if (user.status == "verified")
                    return user;

                // An error occured
                else throw new Exception("Sling returned an error: " + status);
            }
            else throw new Exception("No response was received from the Sling API");
        }

        public static List<ViewModels.SlingArticlesView> GetArticles(string channel)
        {
            string apiUrl = "https://api.sling.is/v1/channels/" + channel + "/articles",
                resultString = "";

            HttpWebRequest request = WebRequest.Create(apiUrl) as HttpWebRequest;
            request.Method = "GET";
            request.Headers.Add("Authorization", apiKey);
            request.Accept = "application/json";

            using (WebResponse webResponse = request.GetResponse())
            using (Stream stream = webResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                resultString = reader.ReadToEnd();

            if (!String.IsNullOrEmpty(resultString))
            {
                var result = JArray.Parse(resultString);

                List<ViewModels.SlingArticlesView> articles = new List<ViewModels.SlingArticlesView>();

                foreach(var a in result)
                {
                    ViewModels.SlingArticlesView article = new ViewModels.SlingArticlesView();
                    article.PostContent = a["content"].ToString();
                    article.Posted = Convert.ToDateTime(a["posted"].ToString());
                    article.PostId = a["id"].ToString();
                    article.UserId = a["user"]["id"].ToString();
                    article.PostedBy = GetUserName(article.UserId);

                    // get commentlist and append
                    article.PostComments = GetPostComments(article.PostId, channel);

                    articles.Add(article);
                }

                

                return articles;
}
            else throw new Exception("No response was received from the Sling API");
        }

        public static string GetUserName(string userid)
        {
            string apiUrl = "https://api.sling.is/v1/users?ids=" + userid,
                resultString = "";

            HttpWebRequest request = WebRequest.Create(apiUrl) as HttpWebRequest;
            request.Method = "GET";
            request.Headers.Add("Authorization", apiKey);
            request.Accept = "application/json";

            using (WebResponse webResponse = request.GetResponse())
            using (Stream stream = webResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                resultString = reader.ReadToEnd();

            if (!String.IsNullOrEmpty(resultString))
            {
                var result = JArray.Parse(resultString);
                var user = result[0];
                string name = user["name"].ToString() + " " + user["lastname"].ToString();

                return name;
            }
            else throw new Exception("No response was received from the Sling API");
        }

        public static string GetPostComments(string postId, string channel)
        {
            string apiUrl = "https://api.sling.is/v1/channels/" + channel + "/articles/" + postId + "/comments",
                resultString = "";

            HttpWebRequest request = WebRequest.Create(apiUrl) as HttpWebRequest;
            request.Method = "GET";
            request.Headers.Add("Authorization", apiKey);
            request.Accept = "application/json";

            using (WebResponse webResponse = request.GetResponse())
            using (Stream stream = webResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                resultString = reader.ReadToEnd();

            if (!String.IsNullOrEmpty(resultString))
            {
                var result = JArray.Parse(resultString);

                /*
                List<Sling.SlingArticleComments> comments = new List<SlingArticleComments>();
                foreach (var c in result)
                {
                    Sling.SlingArticleComments comment = new Sling.SlingArticleComments();
                    comment.commentContent = c["content"].ToString();
                    comment.commentTime = Convert.ToDateTime(c["posted"].ToString());
                    comment.commenterName = GetUserName(c["user"]["id"].ToString());
                    comments.Add(comment);
                }
                */
                
                StringBuilder Sb = new StringBuilder();
                for (int i = 0; i<result.Count; i++)
                {
                    var c = result.ElementAt(i);
                    var content = c["content"].ToString();
                    //var time = Convert.ToDateTime(c["posted"].ToString());
                    var name = GetUserName(c["user"]["id"].ToString());

                    Sb.Append(content + "<br/>" + "- " + name);

                    if (i < result.Count-1) // not the last item
                    {
                        Sb.Append("<br/><br/>");
                    }
                }

                var comments = Sb.ToString();
                return comments;
            }
            else throw new Exception("No response was received from the Sling API");
        }
    }
}