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

namespace Check_Out_App_ULC.Models
{
    public class Sling
    {
        public static readonly string apiKey = "edd54f74a1006c597ebd575713582ac5";

        public class AccountLoginObject
        {
            public string email;
            public string password;
            public string snsPlatform;
            public string snsToken;
        }

        public static string Login(string e, string pw, string snsP, string snsT)
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
                string status = result["meta"]["httpStatus"].ToString();

                // 200 - OK : Continue parsing
                if (status.Contains("200"))
                    return result["result"]["id"].ToString();

                // An error occured
                else throw new Exception("Sling returned an error: " + status);
            }
            else throw new Exception("No response was received from the Sling API");

        }

    }
}