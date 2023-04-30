using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Utility;
using MakerShop.Orders;
using MakerShop.Catalog;
using System.Text;
using System;
using System.IO;

namespace MakerShop.Messaging
{

    public partial class EmailTemplateTrigger_Catalog
    {
        private string _Name;
        public string Name
        {
            get
            {
                if (_Name == null)
                {
                    switch (this.CatalogNodeType)
                    {
                        case MakerShop.Catalog.CatalogNodeType.Category:
                            _Name = CategoryDataSource.Load(this.CatalogNodeId).Name;
                            break;
                        case MakerShop.Catalog.CatalogNodeType.Product:
                            _Name = ProductDataSource.Load(this.CatalogNodeId).Name;
                            break;
                        default:
                            _Name = "ERROR";
                            break;
                    }
                }
                return _Name;
            }
        }

        public Catalog.CatalogNodeType CatalogNodeType
        {
            get
            {
                return (Catalog.CatalogNodeType)this.CatalogNodeTypeId;
            }
        }
        public string CatalogNodeTypeName
        {
            get
            {
                return Enum.GetName(typeof(CatalogNodeType), CatalogNodeType);
            }
        }
        
    }
}