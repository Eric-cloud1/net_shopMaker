using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Catalog
{
    /// <summary>
    /// Interface that is to be implemented by URL Rewrite providers
    /// </summary>
    public interface IUrlRewriter
    {
        /// <summary>
        /// Returns the rewritten URL for the given source URL
        /// </summary>
        /// <param name="sourceUrl">The given source URL</param>
        /// <returns>The rewritten URL</returns>
        string RewriteUrl(string sourceUrl);
    }
}
