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

        public class GetAnnouncementsObject
        {
            public int before;
            public int since;
            public int pagesize;
        }

        public class SlingUser
        {
            public string status { get; set; }
            public string name { get; set; }
        }

        public class SlingArticle
        {
            public string content { get; set; }
            public DateTime posted { get; set; }
            public string id { get; set; }
            //public string commentlist { get; set; }
            public string userid { get; set; }
        }
        
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

        public static JArray GetAnnouncements()
        {
            string apiUrl = "https://api.sling.is/v1/announcements",
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
                //resultString = resultString.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });
                var result = JArray.Parse(resultString);

                return result;
            }
            else throw new Exception("No response was received from the Sling API");
        }

        public static List<SlingArticle> GetArticles()
        {
            string apiUrl = "https://api.sling.is/v1/channels/0/articles",
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

                List<SlingArticle> articles = new List<SlingArticle>();//

                //ViewModels.SlingArticlesView view = new ViewModels.SlingArticlesView();

                foreach(var a in result)
                {
                    SlingArticle article = new SlingArticle();
                    article.content = a["content"].ToString();
                    article.posted = Convert.ToDateTime(a["posted"].ToString());
                    article.id = a["id"].ToString();
                    article.userid = a["user"]["id"].ToString();
                    //article.commentlist = a["commentlist"].ToString();
                    articles.Add(article);
                    
                }

                // get commentlist and append


                // match userid to username and append


                return articles;
            }
            else throw new Exception("No response was received from the Sling API");
        }
    }
}