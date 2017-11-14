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

    public class SmartWaiver
    {
        private static readonly string apiKey = "1358f518a45966dfca6484341e46b346";

        public class SearchRequestData
        {
            public string guid { get; set; }
            public int count { get; set; }
            public int pages { get; set; }
            public int pageSize { get; set; }
        }

        public class Waiver
        {
            public string waiverId { get; set; }
            public DateTime createdOn { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public int numPhotos { get; set; }
            public List<Photo> photos { get; set; }
            public List<string> tags { get; set; }
        }
        
        public class Photo
        {
            public string date { get; set; }
            public string fileType { get; set; }
            public string photoId { get; set; }
            public string photo { get; set; }
        }
        
        public static string GetPingPong(string s)
        {
            string apiUrl = "https://api.smartwaiver.com/" + s,
                resultString = "";

            HttpWebRequest request = WebRequest.Create(apiUrl) as HttpWebRequest;
            request.Method = "GET";
            //request.Headers.Add("Authorization", apiKey);
            request.Accept = "application/json";

            using (WebResponse webResponse = request.GetResponse())
            using (Stream stream = webResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                resultString = reader.ReadToEnd();

            if (!String.IsNullOrEmpty(resultString))
            {
                //var result = JObject.Parse(resultString);

                return resultString;
            }
            else throw new Exception("No response was received from the SmartWaiver API");
        }

        public static SearchRequestData SearchRequest()
        {
            string apiUrl = "https://api.smartwaiver.com/v4/search",
                resultString = "";

            HttpWebRequest request = WebRequest.Create(apiUrl) as HttpWebRequest;
            request.Method = "GET";
            request.Headers.Add("sw-api-key", apiKey);
            request.Accept = "application/json";

            using (WebResponse webResponse = request.GetResponse())
            using (Stream stream = webResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                resultString = reader.ReadToEnd();

            if (!String.IsNullOrEmpty(resultString))
            {
                var result = JObject.Parse(resultString);
                SmartWaiver.SearchRequestData searchRequestData = new SmartWaiver.SearchRequestData();

                searchRequestData.guid = result["search"]["guid"].ToString();
                searchRequestData.count = result["search"]["count"].ToObject<int>();
                searchRequestData.pages = result["search"]["pages"].ToObject<int>();
                searchRequestData.pageSize = result["search"]["pageSize"].ToObject<int>();

                return searchRequestData;
            }
            else throw new Exception("No response was received from the SmartWaiver API");
        }

        public static Waiver GetWaiver(string waiverId)
        {
            string apiUrl = "https://api.smartwaiver.com/v4/waivers/" + waiverId,
                resultString = "";

            HttpWebRequest request = WebRequest.Create(apiUrl) as HttpWebRequest;
            request.Method = "GET";
            request.Headers.Add("sw-api-key", apiKey);
            request.Accept = "application/json";

            using (WebResponse webResponse = request.GetResponse())
            using (Stream stream = webResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                resultString = reader.ReadToEnd();

            if (!String.IsNullOrEmpty(resultString))
            {
                var result = JObject.Parse(resultString);
                Waiver w = new Waiver();
                w.numPhotos = result["waiver"]["photos"].ToObject<int>();
                w.tags = result["waiver"]["tags"].ToObject<List<string>>();
                return w;
            }
            else throw new Exception("No response was received from the SmartWaiver API");
        }

        public static List<Photo> GetPhotos(string waiverId)
        {
            string apiUrl = "https://api.smartwaiver.com/v4/waivers/" + waiverId + "/photos",
                resultString = "";

            HttpWebRequest request = WebRequest.Create(apiUrl) as HttpWebRequest;
            request.Method = "GET";
            request.Headers.Add("sw-api-key", apiKey);
            request.Accept = "application/json";

            using (WebResponse webResponse = request.GetResponse())
            using (Stream stream = webResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                resultString = reader.ReadToEnd();

            if (!String.IsNullOrEmpty(resultString))
            {
                var result = JObject.Parse(resultString);
                var photoArray = result["photos"]["photos"];

                List<Photo> photos = new List<Photo>();
                foreach (var p in photoArray)
                {
                    Photo ph = new Photo();
                    ph.date = p["date"].ToString();
                    ph.fileType = p["fileType"].ToString();
                    ph.photoId = p["photoId"].ToString();
                    ph.photo = p["photo"].ToString();
                    photos.Add(ph);
                }
                return photos;
            }
            else throw new Exception("No response was received from the SmartWaiver API");
        }

        public static List<Waiver> GetSignedWaivers ()
        {
            string apiUrl = "https://api.smartwaiver.com/v4/waivers",
                resultString = "";

            HttpWebRequest request = WebRequest.Create(apiUrl) as HttpWebRequest;
            request.Method = "GET";
            request.Headers.Add("sw-api-key", apiKey);
            request.Accept = "application/json";

            using (WebResponse webResponse = request.GetResponse())
            using (Stream stream = webResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                resultString = reader.ReadToEnd();

            if (!String.IsNullOrEmpty(resultString))
            {
                var resultObject = JObject.Parse(resultString);
                var result = resultObject["waivers"];
                List<Waiver> signedWaivers = new List<Waiver>();

                foreach (var w in result)
                {
                    Waiver wv = new Waiver();

                    wv.waiverId = w["waiverId"].ToString();
                    wv.createdOn = w["createdOn"].ToObject<DateTime>();
                    wv.firstName = w["firstName"].ToString();
                    wv.lastName = w["lastName"].ToString();
                    Waiver wvDetails = GetWaiver(wv.waiverId);
                    wv.numPhotos = wvDetails.numPhotos;
                    wv.tags = wvDetails.tags;
                    wv.photos = GetPhotos(wv.waiverId);

                    signedWaivers.Add(wv);
                }
                return signedWaivers;
            }
            else throw new Exception("No response was received from the SmartWaiver API");
        }

    }
}