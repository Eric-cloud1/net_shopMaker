using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Products;
using MakerShop.Common;

namespace MakerShop.Reporting
{
    /// <summary>
    /// Class that holds summary data about a product sales
    /// </summary>
    public class ProductSummary
    {
        private int _ProductId;
        private string _Name;
        private int _TotalQuantity;
        private LSDecimal _TotalPrice;
        private Product _Product;

        /// <summary>
        /// Id of the product
        /// </summary>
        public int ProductId
        {
            get { return _ProductId; }
            set { _ProductId = value; }
        }

        /// <summary>
        /// Name of the product
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// Total quantity
        /// </summary>
        public int TotalQuantity
        {
            get { return _TotalQuantity; }
            set { _TotalQuantity = value; }
        }

        /// <summary>
        /// Total price
        /// </summary>
        public LSDecimal TotalPrice
        {
            get { return _TotalPrice; }
            set { _TotalPrice = value; }
        }

        /// <summary>
        /// The product object
        /// </summary>
        public Product Product
        {
            get
            {
                if (this._Product == null)
                {
                    this._Product = ProductDataSource.Load(this.ProductId);
                }
                return this._Product;
            }
        }
    }
}
