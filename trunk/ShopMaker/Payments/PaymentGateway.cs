

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Payments.Providers;
using MakerShop.Utility;

using System.Web;

namespace MakerShop.Payments
{
    public partial class PaymentGateway
    {

        /// <summary>
        /// Get SubAffiliates 
        /// </summary>
        /// <returns>A DataSet objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static DataTable GetGateways()
        {

            DataSet Gateways = new DataSet();
            Database database = Token.Instance.Database;

            using (System.Data.Common.DbCommand dataSetCommand = database.GetStoredProcCommand("dll_Gateways"))
            {
                Gateways = database.ExecuteDataSet(dataSetCommand);
            }

            return Gateways.Tables[0];
        }



        /// <summary>
        /// Updates the configuration data for this payment gateway
        /// </summary>
        /// <param name="ConfigData">The configuration data to use</param>
        public void UpdateConfigData(Dictionary<string, string> ConfigData)
        {
            StringBuilder configBuilder = new StringBuilder();
            //urlencode the dictionary
            foreach (string key in ConfigData.Keys)
            {
                if (configBuilder.Length > 0) configBuilder.Append("&");
                configBuilder.Append(key + "=" + System.Web.HttpUtility.UrlEncode(ConfigData[key]));
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
            if (!string.IsNullOrEmpty(this.ConfigData)) {
                string[] pairs = EncryptionHelper.DecryptAES(this.ConfigData).Split("&".ToCharArray());
                foreach (string thisPair in pairs)
                {
                    if (!string.IsNullOrEmpty(thisPair) && thisPair.Contains("=")) {
                        string[] ConfigDataItem = thisPair.Split("=".ToCharArray());
                        string key = ConfigDataItem[0];
                        string keyValue = System.Web.HttpUtility.UrlDecode(ConfigDataItem[1]);
                        if (!string.IsNullOrEmpty(key))
                        {
                            ConfigData.Add(key, keyValue);
                        }
                    }
                }
            }
            return ConfigData;
        }

        /// <summary>
        /// Gateway Suffix
        /// </summary>
        public string Suffix
        {
            get
            {
                try
                {
                    return ParseConfigData()["GatewaySuffix"];
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Billing Statement
        /// </summary>
        public string BillingStatement
        {
            get
            {
                try
                {
                    return ParseConfigData()["BillingStatement"];
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Customer Service Phone Number
        /// </summary>
        public string PhoneNumber
        {
            get
            {
                try
                {
                    return ParseConfigData()["PhoneNumber"];
                }
                catch
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Gets the instance of the provider implementation for this payment gateway
        /// </summary>
        /// <returns>An instance of the provider implementation</returns>
        public IPaymentProvider GetInstance()
        {
            IPaymentProvider instance;
            try
            {
                instance = Activator.CreateInstance(Type.GetType(this.ClassId)) as IPaymentProvider;
            }
            catch
            {
                instance = null;
            }
            if (instance != null)
            {
                instance.Initialize(this.PaymentGatewayId, this.ParseConfigData());
            }
            return instance;
        }
    }
}
