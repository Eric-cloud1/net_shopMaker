using System.ComponentModel;

namespace MakerShop.Products
{
    /// <summary>
    /// This class represents a RelatedProduct object in the database.
    /// </summary>
    public partial class RelatedProduct
    {
        private Product _ChildProduct;

        /// <summary>
        /// The child product
        /// </summary>
        public Product ChildProduct
        {
            get
            {
                if (!this.ChildProductLoaded)
                {
                    this._ChildProduct = ProductDataSource.Load(this.ChildProductId);
                }
                return this._ChildProduct;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool ChildProductLoaded { get { return ((this._ChildProduct != null) && (this._ChildProduct.ProductId == this.ChildProductId)); } }
    }
}
