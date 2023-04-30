using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Catalog
{
    public abstract class UrlRewriterBase : IUrlRewriter, IUrlGenerator
    {
        abstract public string GetBrowseUrl(int categoryId, string name);
        abstract public string GetBrowseUrl(int nodeId, CatalogNodeType nodeType, string name);
        abstract public string GetBrowseUrl(object dataItem);
        abstract public string GetBrowseUrl(int categoryId, int nodeId, CatalogNodeType nodeType, string name);
        abstract public string RewriteUrl(string sourceUrl);
    }
}
