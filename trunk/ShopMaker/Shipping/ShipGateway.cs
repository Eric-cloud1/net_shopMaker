using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Shipping.Providers;
using System.Web;
using MakerShop.Utility;

namespace MakerShop.Shipping
{
    /// <summary>
    /// Class that represents a shipping gateway
    /// </summary>
    public partial class ShipGateway
    {
        /// <summary>
        /// Updates the configuration data for this shipping gateway
        /// </summary>
        /// <param name="ConfigData">The configuration data to use</param>
        public void UpdateConfigData(Dictionary<string, string> ConfigData)
        {
            StringBuilder configBuilder = new StringBuilder();
            //urlencode the dictionary
            foreach (string key in ConfigData.Keys)
            {
                if (configBuilder.Length > 0) configBuilder.Append("&");
                configBuilder.Append(key + "=" + HttpUtility.UrlEncode(ConfigData[key]));
            }
            this.ConfigData = EncryptionHelper.EncryptAES(configBuilder.ToString());
        }

        /// <summary>
        /// Parses the configuration data from a string (as saved in database) to name value pairs
        /// </summary>
        /// <returns>The parsed configuration data</returns>
        public Dictionary<string, string> ParseConfigData()
        {
            Dictionary<string, string> ConfigData = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(this.ConfigData))
            {
                string[] pairs = EncryptionHelper.DecryptAES(this.ConfigData).Split("&".ToCharArray());
                foreach (string thisPair in pairs)
                {
                    if (!string.IsNullOrEmpty(thisPair) && thisPair.Contains("="))
                    {
                        string[] ConfigDataItem = thisPair.Split("=".ToCharArray());
                        string key = ConfigDataItem[0];                        
                        if (!string.IsNullOrEmpty(key) & ConfigDataItem.Length > 1)
                        {
                            string keyValue = HttpUtility.UrlDecode(ConfigDataItem[1]);
                            ConfigData.Add(key, keyValue);
                        }
                    }
                }
            }
            return ConfigData;
        }

        /// <summary>
        /// Gets the instance of the provider implementation for this shipping gateway
        /// </summary>
        /// <returns>An instance of the provider implementation</returns>
        public IShippingProvider GetProviderInstance()
        {
            IShippingProvider instance;
            try
            {
                instance = Activator.CreateInstance(Type.GetType(this.ClassId)) as IShippingProvider;
            }
            catch
            {
                instance = null;
            }
            if (instance != null)
            {
                instance.Initialize(this.ShipGatewayId, this.ParseConfigData());
            }
            return instance;
        }
    }
}
