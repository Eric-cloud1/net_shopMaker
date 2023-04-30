using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using MakerShop.Orders;
using MakerShop.Users;
using System.Xml;
using System.Net;
using System.Web;

namespace MakerShop.Shipping.Providers
{
    /// <summary>
    /// Base class for shipping provider implementations
    /// </summary>
    public abstract class ShippingProviderBase : IShippingProvider
    {

        #region IShippingProvider Members

        private int _ShipGatewayId;
        /// <summary>
        /// Id of the shipping gateway in database. Id is passed at the time of initialization.
        /// </summary>
        public int ShipGatewayId
        {
            get { return _ShipGatewayId; }
            set { _ShipGatewayId = value; }
        }

        /// <summary>
        /// Name of the shipping provider implementation
        /// </summary>
        public abstract string Name { get;}

        /// <summary>
        /// Version of the shipping provider implementation
        /// </summary>
        public abstract string Version { get;}

        private bool _UseDebugMode = false;
        /// <summary>
        /// Whether to use debugging mode or not?
        /// </summary>
        public bool UseDebugMode
        {
            get { return _UseDebugMode; }
            set { _UseDebugMode = value; }
        }

        /// <summary>
        /// Initializes the shipping provider implementation. Called by AC at the time of initialization.
        /// </summary>
        /// <param name="shipGatewayId">Id of the shipping gateway in database</param>
        /// <param name="configurationData">Configuration data as name-value pairs</param>
        public virtual void Initialize(int shipGatewayId, Dictionary<string, string> configurationData)
        {
            this._ShipGatewayId = shipGatewayId;
            if (configurationData.ContainsKey("UseDebugMode")) UseDebugMode = bool.Parse(configurationData["UseDebugMode"]);
        }

        /// <summary>
        /// Gets the configuration data as name-value paris
        /// </summary>
        /// <returns>Configuration data as name-value paris</returns>
        public virtual Dictionary<string, string> GetConfigData()
        {
            Dictionary<string, string> configData = new Dictionary<string, string>();
            configData.Add("UseDebugMode", this.UseDebugMode.ToString());
            return configData;
        }

        /// <summary>
        /// Gets a list of services provided by the shipping provider implementation
        /// </summary>
        /// <returns>An array of ListItem objects containing services provided by the shipping provider implementation</returns>
        public abstract ListItem[] GetServiceListItems();

        /// <summary>
        /// Gets a shipping rate quote for shipping the given basket items from given warehouse to the given destination address
        /// </summary>
        /// <param name="origin">Warehouse from where shipment initiates</param>
        /// <param name="destination">The destination address for the shipment</param>
        /// <param name="contents">Contents of the shipment</param>
        /// <param name="serviceCode">Service code to get rate quote for</param>
        /// <returns>ShipRateQuote for the given shipping requirements</returns>
        public abstract ShipRateQuote GetShipRateQuote(Warehouse origin, Address destination, BasketItemCollection contents, string serviceCode);

        /// <summary>
        /// Gets a shipping rate quote for shipping the given shipment
        /// </summary>
        /// <param name="shipment">The shipment to get rate quote for</param>
        /// <param name="serviceCode">Service code to get rate quote for</param>
        /// <returns>ShipRateQuote for the given shipment</returns>
        public virtual ShipRateQuote GetShipRateQuote(IShipment shipment, string serviceCode)
        {
            return this.GetShipRateQuote(shipment.Warehouse, shipment.Address, shipment.GetItems(), serviceCode);
        }

        /// <summary>
        /// Gets tracking summary for a given tracking number
        /// </summary>
        /// <param name="trackingNumber">The tracking number to get tracking summary for</param>
        /// <returns>Tracking summary </returns>
        public abstract TrackingSummary GetTrackingSummary(TrackingNumber trackingNumber);

        /// <summary>
        /// Gets a Url for the logo of the shipping provider implementation
        /// </summary>
        /// <param name="cs">ClientScriptManager</param>
        /// <returns>A Url for the logo of the shipping provider implementation</returns>
        public abstract string GetLogoUrl(System.Web.UI.ClientScriptManager cs);

        /// <summary>
        /// Gets a Url for the configuration of the shipping provider implementation
        /// </summary>
        /// <param name="cs">ClientScriptManager</param>
        /// <returns>A Url for the configuration of the shipping provider implementation</returns>
        public abstract string GetConfigUrl(System.Web.UI.ClientScriptManager cs);

        /// <summary>
        /// Description of the shipping provider implementation
        /// </summary>
        public abstract string Description { get; }

        #endregion

        /// <summary>
        /// Records communication in App_Data/Logs/ folder
        /// </summary>
        /// <param name="providerName">Name of the provider used for log file name</param>
        /// <param name="direction">Indicates whether this is for data being sent to received from the provider</param>
        /// <param name="message">The message data to record</param>
        protected void RecordCommunication(string providerName, CommunicationDirection direction, string message)
        {
            //GET LOG DIRECTORY
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Logs\\");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            string fileName = Path.Combine(directory, providerName + ".Log");
            using (StreamWriter sw = File.AppendText(fileName))
            {
                sw.WriteLine(direction.ToString() + ": " + message);
                sw.WriteLine(string.Empty);
                sw.Close();
            }
        }

        /// <summary>
        /// Posts an XML request to given URL using the given encoding
        /// </summary>
        /// <param name="request">The XML document to post</param>
        /// <param name="url">The URL to post to</param>
        /// <param name="encoding">The encoding to use</param>
        /// <returns>XML response from the server</returns>
        protected XmlDocument SendRequestToProvider(XmlDocument request, string url, string encoding)
        {
            //EXECUTE WEB REQUEST, SET RESPONSE
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                string referer = context.Request.ServerVariables["HTTP_REFERER"];
                if (!string.IsNullOrEmpty(referer)) httpWebRequest.Referer = referer;
            }

            byte[] requestBytes = System.Text.Encoding.GetEncoding(encoding).GetBytes(request.OuterXml);
            httpWebRequest.ContentLength = requestBytes.Length;
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }
            //READ RESPONSEDATA FROM STREAM
            string responseData;
            using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.GetEncoding(encoding)))
            {
                responseData = responseStream.ReadToEnd();
                responseStream.Close();
            }

            //LOAD RESPONSE INTO XML DOCUMENT
            XmlDocument providerResponse = new XmlDocument();
            providerResponse.LoadXml(responseData);

            //RETURN THE RESPONSE DOCUMENT
            return providerResponse;
        }

        /// <summary>
        /// Posts a string request to the given URL using given encoding
        /// </summary>
        /// <param name="request">The string data to post</param>
        /// <param name="url">The URL to post to</param>
        /// <param name="encoding">The encoding to use</param>
        /// <returns>Response from the server</returns>
        protected string SendRequestToProvider(string request, string url, string encoding)
        {
            //EXECUTE WEB REQUEST, SET RESPONSE
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                string referer = context.Request.ServerVariables["HTTP_REFERER"];
                if (!string.IsNullOrEmpty(referer)) httpWebRequest.Referer = referer;
            }

            byte[] requestBytes = System.Text.Encoding.GetEncoding(encoding).GetBytes(request);
            httpWebRequest.ContentLength = requestBytes.Length;
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }
            //READ RESPONSEDATA FROM STREAM
            string responseData;
            using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.GetEncoding(encoding)))
            {
                responseData = responseStream.ReadToEnd();
                responseStream.Close();
            }

            //RETURN THE RESPONSE 
            return responseData;
        }
        
        /// <summary>
        /// Enumeration that represents the direction of communication between AC and the gateway
        /// </summary>
        public enum CommunicationDirection : int 
        { 
            /// <summary>
            /// Data is sent to the gateway
            /// </summary>
            Send, 
            
            /// <summary>
            /// Data is received from the gateway
            /// </summary>
            Receive 
        };


    }
}
