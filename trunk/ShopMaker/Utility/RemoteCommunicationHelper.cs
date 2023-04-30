using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace MakerShop.Utility
{
    public class RemoteCommunicationHelper
    {
        /// <summary>
        /// HTTP Post
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="requestData"></param>
        /// <returns>response</returns>
        public static string HTTPPost(string URL, string requestData)
        {


            //EXECUTE WEB REQUEST, SET RESPONSE
            string response;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpWebRequest.Timeout = 36000;
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(requestData);
            httpWebRequest.ContentLength = requestBytes.Length;
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }
            using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.UTF8))
            {
                response = responseStream.ReadToEnd();
                responseStream.Close();
            }
            return response;
        }
     
        /// <summary>
        /// HTTP Get
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="requestData"></param>
        /// <returns>response</returns>
        public static string HTTPGet(string URL, string requestData)
        {


            //EXECUTE WEB REQUEST, SET RESPONSE
            System.Net.WebRequest wr = System.Net.WebRequest.Create(URL + "?" + requestData);
            wr.Method = "GET";
           // wr.Timeout = TimeOut;

            System.Net.WebResponse wrsp = wr.GetResponse();

            System.IO.Stream s = wrsp.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(s);
            return sr.ReadToEnd();
        }
        /// <summary>
        /// HTTP Get
        /// </summary>
        /// <param name="URI"></param>
        /// <returns>response</returns>
        public static string HTTPGet(string URI)
        {


            //EXECUTE WEB REQUEST, SET RESPONSE
            System.Net.WebRequest wr = System.Net.WebRequest.Create(URI);
            wr.Method = "GET";
            // wr.Timeout = TimeOut;

            System.Net.WebResponse wrsp = wr.GetResponse();

            System.IO.Stream s = wrsp.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(s);
            return sr.ReadToEnd();
        }
    }
}
