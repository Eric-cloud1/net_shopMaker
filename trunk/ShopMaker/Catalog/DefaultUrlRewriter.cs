using System;
using System.Web;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using MakerShop.Catalog;
using MakerShop.Utility;
using MakerShop.Products;

namespace MakerShop.Catalog
{
    /// <summary>
    /// Default implementation of IUrlRewriter
    /// </summary>
    public class DefaultUrlRewriter : IUrlRewriter
    {
        private const string REWRITE_PATTERN = "(.*)/.*-(C|c|P|p|W|w|L|l)(\\d+)(?:[Cc](\\d+))?\\.aspx(?:\\?(.*))?";

        /// <summary>
        /// Returns the rewritten URL for the given source URL
        /// </summary>
        /// <param name="sourceUrl">The given source URL</param>
        /// <returns>The rewritten URL</returns>
        public string RewriteUrl(string sourceUrl)
        {
            // CHECK FOR URL FITTING AC PATTERN
            Match urlMatch;
            urlMatch = Regex.Match(sourceUrl, REWRITE_PATTERN, (RegexOptions.IgnoreCase & RegexOptions.Compiled));
            if (urlMatch.Success)
            {
                // URL FOUND, PARSE AND REWRITE
                string serverPath = (urlMatch.Groups[1].ToString() + "/");
                string objectType = urlMatch.Groups[2].ToString();
                string objectId = urlMatch.Groups[3].ToString();
                string categoryId = urlMatch.Groups[4].ToString();
                string urlParams = urlMatch.Groups[5].ToString();
                string displayPage = string.Empty;
                // DISPLAY PAGE NOT PRESENT, LOOK UP FROM TABLE
                // TODO:LOOKUP DISPLAY PAGE FROM RULES
                // Dim displayPage As String = RewriteUrlLookupEngine.GetDisplayPage(app, objectType, objectId)
                StringBuilder dynamicUrl = new StringBuilder();
                switch (objectType)
                {
                    case "C":
                    case "c":
                        displayPage = CatalogDataSource.GetDisplayPage(AlwaysConvert.ToInt(objectId), CatalogNodeType.Category);
                        dynamicUrl.Append((serverPath
                                        + (displayPage + ("?CategoryId=" + objectId))));
                        break;
                    case "P":
                    case "p":
                        displayPage = CatalogDataSource.GetDisplayPage(AlwaysConvert.ToInt(objectId), CatalogNodeType.Product);
                        dynamicUrl.Append((serverPath
                                        + (displayPage + ("?ProductId=" + objectId))));
                        if ((categoryId.Length > 0))
                        {
                            dynamicUrl.Append(("&CategoryId=" + categoryId));
                        }
                        break;
                    case "W":
                    case "w":
                        displayPage = CatalogDataSource.GetDisplayPage(AlwaysConvert.ToInt(objectId), CatalogNodeType.Webpage);
                        dynamicUrl.Append((serverPath
                                        + (displayPage + ("?WebpageId=" + objectId))));
                        if ((categoryId.Length > 0))
                        {
                            dynamicUrl.Append(("&CategoryId=" + categoryId));
                        }
                        break;
                    case "L":
                    case "l":
                        displayPage = CatalogDataSource.GetDisplayPage(AlwaysConvert.ToInt(objectId), CatalogNodeType.Link);
                        dynamicUrl.Append((serverPath
                                        + (displayPage + ("?LinkId=" + objectId))));
                        if ((categoryId.Length > 0))
                        {
                            dynamicUrl.Append(("&CategoryId=" + categoryId));
                        }
                        break;
                }
                if ((urlParams.Length > 0))
                {
                    dynamicUrl.Append(("&" + urlParams));
                }
                return dynamicUrl.ToString();
            }
            return sourceUrl;
        }
    }
}
