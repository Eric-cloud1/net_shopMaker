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
    /// This class represents a Link object in the database.
    /// </summary>
    public partial class Link : CatalogableBase
    {
        string _NavigateUrl = string.Empty;
        /// <summary>
        /// Navigation URL of this Link object
        /// </summary>
        public override string NavigateUrl
        {
            get
            {
                if (_NavigateUrl.Length == 0)
                {
                    _NavigateUrl = UrlGenerator.GetBrowseUrl(this.LinkId, CatalogNodeType.Link, this.Name);
                }
                return _NavigateUrl;
            }
        }

        private LinkCategoryCollection _Categories;
        /// <summary>
        /// Categories this Link object is associated to
        /// </summary>
        public LinkCategoryCollection Categories
        {
            get
            {
                if (_Categories == null)
                {
                    _Categories = new LinkCategoryCollection();
                    _Categories.Load(this.LinkId);
                }
                return _Categories;
            }
        }

        /// <summary>
        /// Save this Link object to database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public SaveResult Save()
        {
            SaveResult tempResult = this.BaseSave();
            if (tempResult != SaveResult.Failed)
            {
                if (_Categories != null)
                {
                    _Categories.LinkId = this.LinkId;
                    _Categories.Save();
                }
            }
            return tempResult;
        }

        /// <summary>
        /// Delete this Link object from database
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public bool Delete()
        {
            //DELETE ALL CATALOG NODES LINKED FROM THIS WEBPAGE
            Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);

            DbCommand deleteCommand = database.GetSqlStringCommand("DELETE FROM ac_CatalogNodes WHERE CatalogNodeId=@linkId AND CatalogNodeTypeId=3");
            database.AddInParameter(deleteCommand, "@linkId", DbType.Int32, this.LinkId);
            database.ExecuteNonQuery(deleteCommand);

            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            //CALL THE BASE DELETE METHOD
            return this.BaseDelete();
        }

        /// <summary>
        /// Create a copy of the given Link object
        /// </summary>
        /// <param name="linkId">Id of the Link object to create copy of</param>
        /// <returns>Copy of the given Link object</returns>
        public static Link Copy(int linkId)
        {
            Link copy = LinkDataSource.Load(linkId, false);
            if (copy != null)
            {
                copy.LinkId = 0;
                return copy;
            }
            return null;
        }

    }
}
