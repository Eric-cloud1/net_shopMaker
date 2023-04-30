using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Common;
using System.IO;
using System.Xml;
using MakerShop.Utility;

namespace MakerShop.DataClient.Api.Schema
{
    public partial class ACAuthenticationResponse
    {
        /// <summary>
        /// Initializes the VersionInfo data.
        /// Should only be called on server side.
        /// </summary>
        public void InitVersionInfo()
        {
            // ONLY INITIALIZE ON SERVER
            try
            {
                if (Token.Instance != null && Token.Instance.Store != null)
                {
                    // VERSION INFORMATION
                    this.VersionInfo = new VersionInfo();
                    this.VersionInfo.StoreName = Token.Instance.Store.Name;
                    this.VersionInfo.ClientApiVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    this.VersionInfo.DataPortVersion = Properties.Resources.DataPortVersion;

                    // DEFAULT INITIALIZATION
                    this.VersionInfo.StoreVersion = "7.0";
                    this.VersionInfo.Platform = "ASP.NET";

                    String filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\VersionInfo.xml");

                    if (File.Exists(filePath))
                    {
                        try
                        {
                            XmlDocument versionDocument = new XmlDocument();

                            versionDocument.Load(filePath);
                            //XmlNode versionInfoNode = versionDocument.DocumentElement.SelectSingleNode("VersionInfo");
                            this.VersionInfo.Platform = XmlUtility.GetElementValue(versionDocument.DocumentElement, "Platform", "ASP.NET");
                            this.VersionInfo.StoreVersion = XmlUtility.GetElementValue(versionDocument.DocumentElement, "Version", "7.0");
                            this.VersionInfo.BuildNumber = XmlUtility.GetElementValue(versionDocument.DocumentElement, "BuildNumber", string.Empty);
                            this.VersionInfo.BuildDate = XmlUtility.GetElementValue(versionDocument.DocumentElement, "BuildDate", string.Empty);

                        }
                        catch
                        {
                            // DO NOTHING
                        }
                    }
                }
            }
            catch { /* DO NOTHING */}
        }
    }
}
