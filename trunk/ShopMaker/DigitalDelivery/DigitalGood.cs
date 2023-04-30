using System;
using MakerShop.Common;
using MakerShop.Data;
using System.Data;
using System.Data.Common;
using System.IO;
using MakerShop.Utility;
using System.Collections.Generic;
using System.Web;
using MakerShop.Orders;

namespace MakerShop.DigitalDelivery
{
    /// <summary>
    /// This class represents a DigitalGood object in the database.
    /// </summary>
    public partial class DigitalGood
    {
        /// <summary>
        /// Activation mode of this digital good
        /// </summary>
        public ActivationMode ActivationMode
        {
            get { return (ActivationMode)this.ActivationModeId; }
            set { this.ActivationModeId = (byte)value; }
        }

        /// <summary>
        /// Fulfillment mode of this digital good
        /// </summary>
        public FulfillmentMode FulfillmentMode
        {
            get { return (FulfillmentMode)this.FulfillmentModeId; }
            set { this.FulfillmentModeId = (byte)value; }
        }

        /// <summary>
        /// License agreement mode of this digital good
        /// </summary>
        public LicenseAgreementMode LicenseAgreementMode
        {
            get { return (LicenseAgreementMode)this.LicenseAgreementModeId; }
            set { this.LicenseAgreementModeId = (byte)value; }
        }

        private string GetFilePath()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                return context.Server.MapPath("~/App_Data/DigitalGoods");
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\DigitalGoods");
        }

        /// <summary>
        /// Absolute file path of the file resource
        /// </summary>
        public string AbsoluteFilePath
        {
            get
            {
                return Path.Combine(GetFilePath(), this.ServerFileName);
            }
        }

        /// <summary>
        /// Reads data of the file for this digital good in the given buffer
        /// </summary>
        /// <param name="buffer">buffer to read data in to</param>
        /// <param name="offset">offset to start reading from</param>
        /// <param name="length">number of bytes to read</param>
        /// <returns>total number of bytes read</returns>
        public int ReadFileData(out byte[] buffer, int offset, int length){
            int bytesRead = 0;
            buffer = new byte[length];
            //GET FILE PATH
            string filePath = Path.Combine(GetFilePath(), this.ServerFileName);
            FileInfo fi = new FileInfo(filePath);
            if ((fi.Exists) && (offset < fi.Length))
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    fileStream.Seek(offset, SeekOrigin.Begin);
                    bytesRead = (int)fileStream.Read(buffer, 0, length);
                    fileStream.Close();
                }
            }
            return bytesRead;
        }

        /// <summary>
        /// Writes byte data to the file resource of this digital good
        /// </summary>
        /// <param name="value">Data to write</param>
        public void WriteFileData(byte[] value)
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                //STORE THE CURRENT STATE OF THE OBJECT
                bool saveIsDirty = this.IsDirty;
                //GET THE PATH TO THE DIGITAL GOODS FOLDER
                string filePath = GetFilePath();
                //MAKE SURE DIRECTORY EXISTS
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                //GENERATE THE FILE NAME IF NEEDED
                if (string.IsNullOrEmpty(this.ServerFileName)) this.ServerFileName = Guid.NewGuid().ToString() + ".esd";
                //WRITE THE FILE DATA
                WriteBinaryFile(Path.Combine(filePath, this.ServerFileName), value);
                //UPDATE THE FILE SIZE
                if (value == null) this.FileSize = 0;
                else this.FileSize = value.Length;
                //UDPATE THE DATABASE RECORD
                Database database = Token.Instance.Database;
                DbCommand updateCommand = database.GetSqlStringCommand("UPDATE ac_DigitalGoods SET FileSize = @fileSize, ServerFileName = @serverFileName WHERE DigitalGoodId = @digitalGoodId");
                database.AddInParameter(updateCommand, "@fileSize", System.Data.DbType.Int32, this.FileSize);
                database.AddInParameter(updateCommand, "@serverFileName", System.Data.DbType.String, NullableData.DbNullify(this.ServerFileName));
                database.AddInParameter(updateCommand, "@digitalGoodId", System.Data.DbType.Int32, this.DigitalGoodId);
                database.ExecuteNonQuery(updateCommand);
                //RESTORE THE STATE OF THE OBJECT
                this.IsDirty = saveIsDirty;
            }
        }

        private void WriteBinaryFile(string filePath, byte[] data)
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            // ENSURE DIRECTORY PATH
            FileInfo fi = new FileInfo(filePath);
            if(!fi.Directory.Exists) fi.Directory.Create();
            using (FileStream fs = File.OpenWrite(filePath))
            {
                fs.Write(data, 0, data.Length);
                fs.Close();
            }
        }

        /// <summary>
        /// Gets expiration date of this digital good
        /// </summary>
        /// <param name="startingDate">Starting date</param>
        /// <param name="timeout">time out</param>
        /// <returns>Expiration date</returns>
        public static DateTime GetExpirationDate(DateTime startingDate, string timeout)
        {
            int days;
            int hours;
            int minutes;
            ParseTimeout(timeout, out days, out hours, out minutes);
            if ((days > 0) || (hours > 0) || (minutes > 0)) return startingDate.Add(new TimeSpan(days, hours, minutes, 0));
            return System.DateTime.MaxValue;
        }

        /// <summary>
        /// Parses a timeout string of the format "days,hours,minutes"
        /// </summary>
        /// <param name="timeout">The timeout string</param>
        /// <param name="days">Number of days</param>
        /// <param name="hours">Number of hours</param>
        /// <param name="minutes">Number of minutes</param>
        public static void ParseTimeout(string timeout, out int days, out int hours, out int minutes)
        {
            days = 0;
            hours = 0;
            minutes = 0;
            if (!string.IsNullOrEmpty(timeout))
            {
                string[] tokens = timeout.Split(",".ToCharArray());
                days = AlwaysConvert.ToInt(tokens[0]);
                hours = AlwaysConvert.ToInt(tokens[1]);
                minutes = AlwaysConvert.ToInt(tokens[2]);
            }
        }

        /// <summary>
        /// Gets an instance of the serial key provider associated with this digital good
        /// </summary>
        /// <returns>Instance of the serial key provider associated with this digital good</returns>
        public ISerialKeyProvider GetSerialKeyProviderInstance()
        {
            ISerialKeyProvider instance;
            try
            {
                instance = Activator.CreateInstance(Type.GetType(SerialKeyProviderId)) as ISerialKeyProvider;
            }
            catch
            {
                instance = null;
            }
            if (instance != null)
            {
                instance.Initialize(this, this.ParseConfigData());
            }
            return instance;
        }

        private Dictionary<string, string> ParseConfigData()
        {
            Dictionary<string, string> ConfigData = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(this.SerialKeyConfigData))
            {
                string[] pairs = this.SerialKeyConfigData.Split("&".ToCharArray());
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
        /// Retuns the size of the resource file associated with this digital good
        /// </summary>
        public string FormattedFileSize
        {
            get
            {
                return FileHelper.FormatFileSize(this.FileSize);
            }
        }

        /// <summary>
        /// Check if the given serial key is one of the serial keys for this digital good
        /// </summary>
        /// <param name="keyData">The key to check</param>
        /// <returns><b>true</b> if the given serial key is one of the serial key for this digital good, <b>false</b> otherwise</returns>
        public bool HasSerialKey(string keyData)
        {
            if(string.IsNullOrEmpty(keyData)) return false;
            if (this.SerialKeys == null || this.SerialKeys.Count == 0)
            {
                return false;
            }

            foreach (SerialKey skey in this.SerialKeys)
            {
                if (keyData.Equals(skey.SerialKeyData))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a copy of the specified digital good
        /// </summary>
        /// <param name="digitalGoodId">Id of the DigitalGood to create copy of</param>
        /// <returns>The created copy of the specified DigitalGood</returns>
        public static DigitalGood Copy(int digitalGoodId)
        {
            DigitalGood copy = DigitalGoodDataSource.Load(digitalGoodId, false);
            if (copy != null)
            {
                copy.DigitalGoodId = 0;
            }
            return copy;
        }

        /// <summary>
        /// Deletes this digital good from database
        /// </summary>
        /// <param name="deleteFile">If <b>true</b> the associated resource file is also deleted</param>
        /// <returns><b>true</b> if delete is successful, <b>false</b> otherwise</returns>
        public virtual bool Delete(bool deleteFile)
        {
            //CHECK WHETHER WE SHOULD DELETE THE FILE
            if (deleteFile)
            {
                try
                {
                    System.IO.File.Delete(this.AbsoluteFilePath);
                }
                catch (Exception ex)
                {
                    Logger.Warn("Failed to delete file " + this.AbsoluteFilePath + " while deleting digital good " + this.Name + ".", ex);
                }
            }
            return this.Delete();
        }
    }
}
