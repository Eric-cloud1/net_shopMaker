using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Products;

namespace MakerShop.Catalog
{
    /// <summary>
    /// Utility class to help create URLs for catalog items
    /// </summary>
    public static class UrlGenerator
    {
        /// <summary>
        /// Gets a browsing URL for given category
        /// </summary>
        /// <param name="categoryId">Id of the category for which to get the browse URL</param>
        /// <param name="name">The name to use in the link</param>
        /// <returns>A browsing URL for the given category</returns>
        public static string GetBrowseUrl(int categoryId, string name)
        {
            return GetBrowseUrl(0, categoryId, CatalogNodeType.Category, name);
        }

        /// <summary>
        /// Gets a browsing URL for the given node
        /// </summary>
        /// <param name="nodeId">The Id of the node for which to get the browse URL</param>
        /// <param name="nodeType">The type of node</param>
        /// <param name="name">The name to use in the link</param>
        /// <returns>A browsing URL for the given node</returns>
        public static string GetBrowseUrl(int nodeId, CatalogNodeType nodeType, string name)
        {            
            if (Token.Instance.UrlGeneratorEnabled)
            {
                IUrlGenerator generator = Token.Instance.UrlGenerator;
                if (generator != null)
                    return generator.GetBrowseUrl(nodeId, nodeType, name);
                else
                    return GetClassicUrl(nodeId, nodeType, name);
            }
            else
            {
                return GetClassicUrl(nodeId, nodeType, name);
            }
        }

        /// <summary>
        /// Generates a browse url from an object reference.
        /// </summary>
        /// <param name="dataItem">Either a Category, Product, or CatalogNode to generate a browse url for.</param>
        /// <returns>The browse url for the object.</returns>
        public static string GetBrowseUrl(Object dataItem)
        {
            Product product = dataItem as Product;
            if (product != null) return GetBrowseUrl(product.ProductId, CatalogNodeType.Product, product.Name);
            Category category = dataItem as Category;
            if (category != null) return GetBrowseUrl(category.CategoryId, CatalogNodeType.Category, category.Name);
            CatalogNode node = dataItem as CatalogNode;
            if (node != null) return GetBrowseUrl(node.CategoryId, node.CatalogNodeId, node.CatalogNodeType, node.Name);
            throw new ArgumentException("dataItem is not of a type supported by this method.", "dataItem");
        }

        /// <summary>
        /// Gets a browsing URL for the given node
        /// </summary>
        /// <param name="categoryId">The category to which the node belongs</param>
        /// <param name="nodeId">The Id of the node for which to get the browse URL</param>
        /// <param name="nodeType">The type of node</param>
        /// <param name="name">The name to use in the link</param>
        /// <returns>A browsing URL for the given node</returns>
        public static string GetBrowseUrl(int categoryId, int nodeId, CatalogNodeType nodeType, string name)
        {
            if (Token.Instance.UrlGeneratorEnabled)
            {
                IUrlGenerator generator = Token.Instance.UrlGenerator;
                if (generator != null)
                    return generator.GetBrowseUrl(categoryId, nodeId, nodeType, name);
                else
                    return GetClassicUrl(categoryId, nodeId, nodeType, name);
            }
            else
            {
                return GetClassicUrl(categoryId, nodeId, nodeType, name);
            }
        }

        private static string GetClassicUrl(int nodeId, CatalogNodeType nodeType, string name)
        {
            //PRODUCE URL WITH ACTUAL DISPLAY PAGE AND QUERY STRING PARAMETERS
            string displayPage;
            if (nodeType != CatalogNodeType.Link) displayPage = CatalogDataSource.GetDisplayPage(nodeId, nodeType);
            else
            {
                Link link = LinkDataSource.Load(nodeId);
                if (link.DisplayPage.Length == 0) return link.TargetUrl;
                else displayPage = link.DisplayPage;
            }
            return string.Format("~/{0}?{1}Id={2}", displayPage, nodeType.ToString(), nodeId);
        }

        private static string GetClassicUrl(int categoryId, int nodeId, CatalogNodeType nodeType, string name)
        {
            //PRODUCE URL WITH ACTUAL DISPLAY PAGE AND QUERY STRING PARAMETERS
            string displayPage = CatalogDataSource.GetDisplayPage(nodeId, nodeType);
            if (nodeType == CatalogNodeType.Category)
            {
                return string.Format("~/{0}?{1}Id={2}", displayPage, nodeType.ToString(), nodeId);
            }
            else
            {
                string[] args = { displayPage, nodeType.ToString(), nodeId.ToString(), categoryId.ToString() };
                return string.Format("~/{0}?{1}Id={2}&CategoryId={3}", args);
            }
        }

        /// <summary>
        /// Strips out all but Space, Digits, Letters A-Z(a-z) and underscore. 
        /// Also reduces the multiple spaces to single space
        /// </summary>
        /// <param name="name">The string to sanitize</param>
        /// <returns>A sanitized string that contains only letters, digits, space, and underscore.</returns>
        private static string SanitizeName(string name)
        {
            // STRIP OUT ALL BUT SPACE, DIGITS, UNICODE LETTERS, AND UNDERSCORE
            // ALSO REDUCE MULTIPLE SPACES TO SINGLE SPACE
            if (string.IsNullOrEmpty(name))
            {
                return String.Empty;
            }
            string tempName = Regex.Replace(name, "[^\\s\\w-]", string.Empty);
            tempName = Regex.Replace(tempName, "[\\s]", "-");
            tempName = Regex.Replace(tempName, "-{2,}", "-");
            return System.Web.HttpUtility.UrlEncode(tempName);
        }
    }
}
