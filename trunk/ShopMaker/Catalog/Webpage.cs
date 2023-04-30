using System;
using System.Collections.Generic;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Catalog;
using System.Data;
using System.Data.Common;
using MakerShop.Shipping;
using MakerShop.Users;
using MakerShop.Utility;

namespace MakerShop.Catalog
{
    /// <summary>
    /// This class represents a Webpage object in the database.
    /// </summary>
    public partial class Webpage : CatalogableBase
    {
        string _NavigateUrl = string.Empty;
        /// <summary>
        /// The navigation URL for this Webpage
        /// </summary>
        public override string NavigateUrl
        {
            get
            {
                if (_NavigateUrl.Length == 0)
                {
                    _NavigateUrl = UrlGenerator.GetBrowseUrl(this.WebpageId, CatalogNodeType.Webpage, this.Name);
                }
                return _NavigateUrl;
            }
        }

        private WebpageCategoryCollection _Categories;
        /// <summary>
        /// Categories this Webpage is associated to
        /// </summary>
        public WebpageCategoryCollection Categories
        {
            get
            {
                if (_Categories == null)
                {
                    _Categories = new WebpageCategoryCollection();
                    _Categories.Load(this.WebpageId);
                }
                return _Categories;
            }
        }

        /// <summary>
        /// Save this Webpage object to the database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public SaveResult Save()
        {
            SaveResult tempResult = this.BaseSave();
            if (tempResult != SaveResult.Failed)
            {
                if (_Categories != null)
                {
                    _Categories.WebpageId = this.WebpageId;
                    _Categories.Save();
                }
            }
            return tempResult;
        }

        /// <summary>
        /// Delete this Webpage object from the database
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public bool Delete()
        {
            //DELETE ALL CATALOG NODES LINKED FROM THIS WEBPAGE
            Database database = Token.Instance.Database;
            DbCommand deleteCommand = database.GetSqlStringCommand("DELETE FROM ac_CatalogNodes WHERE CatalogNodeId=@webpageId AND CatalogNodeTypeId=2");
            database.AddInParameter(deleteCommand, "@webpageId", DbType.Int32, this.WebpageId);

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            database.ExecuteNonQuery(deleteCommand);
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            //CALL THE BASE DELETE METHOD
            return this.BaseDelete();
        }

        /// <summary>
        /// Creates a copy of the specified Webpage
        /// </summary>
        /// <param name="webpageId">Id of the Webpage to create copy of</param>
        /// <returns>The copy object</returns>
        public static Webpage Copy(int webpageId)
        {
            Webpage copy = WebpageDataSource.Load(webpageId, false);
            if (copy != null)
            {
                copy.WebpageId = 0;
                return copy;
            }
            return null;
        }
    }
}
