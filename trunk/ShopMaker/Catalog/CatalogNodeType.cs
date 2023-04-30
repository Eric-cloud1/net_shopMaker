using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Catalog
{
    /// <summary>
    /// Enumeration that represents the type of a CatalogNode
    /// </summary>
    public enum CatalogNodeType : byte
    {
        /// <summary>
        /// CatalogNode is a category
        /// </summary>
        Category, 
        
        /// <summary>
        /// CatalogNode is a product
        /// </summary>
        Product, 
        
        /// <summary>
        /// CatalogNode is a web page
        /// </summary>
        Webpage, 

        /// <summary>
        /// CatalogNode is a link
        /// </summary>
        Link
    }

    /// <summary>
    /// Flag Enumeration for bitwise comparison of CatalogNode types
    /// </summary>
    [Flags] public enum CatalogNodeTypeFlags : int
    {
        /// <summary>
        /// CatalogNode type for none
        /// </summary>
        None = 0, 
        
        /// <summary>
        /// CatalogNode type for a category
        /// </summary>
        Category = 1, 
        
        /// <summary>
        /// CatalogNode type for a product
        /// </summary>
        Product = 2, 
        
        /// <summary>
        /// CatalogNode type for a webpage
        /// </summary>
        Webpage = 4, 
        
        /// <summary>
        /// CatalogNode type for a link
        /// </summary>
        Link = 8, 
        
        /// <summary>
        /// CatalogNode type for a leaf node
        /// </summary>
        Leaf = Product | Webpage | Link, 
        
        /// <summary>
        /// CatalogNode type for any all of category, product, webpage or link
        /// </summary>
        All = Category | Product | Webpage | Link
    }
}
