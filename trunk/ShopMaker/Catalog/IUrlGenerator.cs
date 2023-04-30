using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Catalog;

namespace MakerShop.Catalog
{
    /// <summary>
    /// Interface to be implemented by URL Generator providers
    /// </summary>
    public interface IUrlGenerator
    {
        /// <summary>
        /// Gets a browsing URL for given category
        /// </summary>
        /// <param name="categoryId">Id of the category for which to get the browse URL</param>
        /// <param name="name">The name to use in the link</param>
        /// <returns>A browsing URL for the given category</returns>
        string GetBrowseUrl(int categoryId, string name);

        /// <summary>
        /// Gets a browsing URL for the given node
        /// </summary>
        /// <param name="nodeId">The Id of the node for which to get the browse URL</param>
        /// <param name="nodeType">The type of node</param>
        /// <param name="name">The name to use in the link</param>
        /// <returns>A browsing URL for the given node</returns>
        string GetBrowseUrl(int nodeId, CatalogNodeType nodeType, string name);

        /// <summary>
        /// Generates a browse url from an object reference.
        /// </summary>
        /// <param name="dataItem">Either a Category, Product, or CatalogNode to generate a browse url for.</param>
        /// <returns>The browse url for the object.</returns>
        string GetBrowseUrl(Object dataItem);

        /// <summary>
        /// Gets a browsing URL for the given node
        /// </summary>
        /// <param name="categoryId">The category to which the node belongs</param>
        /// <param name="nodeId">The Id of the node for which to get the browse URL</param>
        /// <param name="nodeType">The type of node</param>
        /// <param name="name">The name to use in the link</param>
        /// <returns>A browsing URL for the given node</returns>
        string GetBrowseUrl(int categoryId, int nodeId, CatalogNodeType nodeType, string name);

    }

}
