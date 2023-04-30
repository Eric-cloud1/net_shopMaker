using System;
using MakerShop.Common;
using MakerShop.Taxes;
using MakerShop.Utility;
using MakerShop.Payments;

namespace MakerShop.Products
{
    /// <summary>
    /// This class represents a ProductCommission object in the database.
    /// </summary>
    public partial class ProductCommission
    {
        private Product _Product;

        /// <summary>
        /// The Product
        /// </summary>
        public Product Product
        {
            get
            {
                if (_Product == null)
                {
                    _Product = ProductDataSource.Load(this.ProductId);
                }
                return _Product;
            }
        }
    }
}