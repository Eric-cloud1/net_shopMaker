namespace MakerShop.Catalog
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MakerShop.Common;
    using MakerShop.Products;

    /// <summary>
    /// This class implements a PersistentCollection of CatalogNode objects.
    /// </summary>
    public partial class CatalogNodeCollection
    {
        /// <summary>
        /// Add a CatalogNode object of type Category to this collection
        /// </summary>
        /// <param name="categoryId">Category Id for the catalog node to add</param>
        /// <param name="category">The category object to add</param>
        public void Add(int categoryId, Category category)
        {
            base.Add(new CatalogNode(categoryId, category.CategoryId, CatalogNodeType.Category, -1, false, category.Visibility, category.Name, category.Summary, category.ThumbnailUrl, category.ThumbnailAltText));
        }

        /// <summary>
        /// Add a CatalogNode object of type Product to this collection
        /// </summary>
        /// <param name="categoryId">Category Id for the catalog node to add</param>
        /// <param name="product">The product object to add</param>
        public void Add(int categoryId, Product product)
        {
            base.Add(new CatalogNode(categoryId, product.ProductId, CatalogNodeType.Product, -1, false, product.Visibility, product.Name, product.Summary, product.ThumbnailUrl, product.ThumbnailAltText));
        }

        /// <summary>
        /// Add a CatalogNode object of type Webpage to this collection
        /// </summary>
        /// <param name="categoryId">Category Id for the catalog node to add</param>
        /// <param name="webpage">The webpage object to add</param>
        public void Add(int categoryId, Webpage webpage)
        {
            base.Add(new CatalogNode(categoryId, webpage.WebpageId, CatalogNodeType.Webpage, -1, false, webpage.Visibility, webpage.Name, webpage.Summary, webpage.ThumbnailUrl, webpage.ThumbnailAltText));
        }

        /// <summary>
        /// Add a CatalogNode object of type Link to this collection
        /// </summary>
        /// <param name="categoryId">Category Id for the catalog node to add</param>
        /// <param name="link">The link object to add</param>
        public void Add(int categoryId, Link link)
        {
            base.Add(new CatalogNode(categoryId, link.LinkId, CatalogNodeType.Link, -1, false, link.Visibility, link.Name, link.Summary, link.ThumbnailUrl, link.ThumbnailAltText));
        }

        /// <summary>
        /// Adds a CatalogNode object to this collection
        /// </summary>
        /// <param name="node">CatalogNode object to add</param>
        new public void Add(CatalogNode node)
        {
            base.Add(node);
        }
    }
}
