using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using MakerShop.Utility;

namespace MakerShop.Catalog
{
    /// <summary>
    /// An implementation of SiteMapProvider. It extends XmlSiteMapProvider.
    /// </summary>
    public class CmsSiteMapProvider : System.Web.XmlSiteMapProvider
    {
        private SiteMapNode GetCatalogNode()
        {
            SiteMapNode catalogNode = base.FindSiteMapNodeFromKey("Catalog");
            return catalogNode;
        }

        /// <summary>
        /// Retrieves a SiteMapNode object that represents the page at the specified URL
        /// </summary>
        /// <param name="rawUrl">The URL for which to retrieve the SiteMapNode object</param>
        /// <returns>A SiteMapNode object corresponding to the given URL</returns>
        public override System.Web.SiteMapNode FindSiteMapNode(string rawUrl)
        {
            //SHORTCUT THE CATALOG SEARCH
            if (rawUrl.ToLowerInvariant().Equals("catalog")) return base.FindSiteMapNode(rawUrl);

            WebTrace.Write(this.GetType().ToString(), "FindSiteMapNode: " + rawUrl + ", Check Regex");
            System.Web.SiteMapNode theNode = null;

            //SEE WHETHER THIS URL MATCHES KNOWN CMS PAGES
            Match urlMatch = Regex.Match(rawUrl, "(?<baseUrl>.*)\\?.*CategoryId=(?<categoryId>[^&]*)", (RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase));
            WebTrace.Write(this.GetType().ToString(), "Cms Node Found: " + urlMatch.Success.ToString());
            if (urlMatch.Success)
            {
                //GET THE BASE NODE
                WebTrace.Write(this.GetType().ToString(), "Check for Catalog Node");
                theNode = GetCatalogNode();
                WebTrace.Write(this.GetType().ToString(), "Catalog Node Found: " + (theNode != null));
                if (theNode != null)
                {
                    //theNode = theNode.Clone(true);
                    theNode.ReadOnly = false;
                    //INJECT THE CMS PATH
                    WebTrace.Write(this.GetType().ToString(), "Cms Match Value: " + urlMatch.Groups[2].Value);
                    List<CmsPathNode> thePath = CmsPath.GetCmsPath(0, AlwaysConvert.ToInt(urlMatch.Groups[2].Value), CatalogNodeType.Category);
                    WebTrace.Write(this.GetType().ToString(), "thePath.Count: " + thePath.Count.ToString());
                    for (int i = 1; i < thePath.Count; i++)
                    {
                        SiteMapNode newNode = new SiteMapNode(theNode.Provider, thePath[i].NodeId.ToString(), thePath[i].Url, thePath[i].Title, thePath[i].Description);
                        newNode.ParentNode = theNode;
                    }
                }
                else
                {
                    theNode = base.FindSiteMapNode(urlMatch.Groups[1].Value);
                    WebTrace.Write(this.GetType().ToString(), "Base Node Found: " + ((bool)(theNode != null)).ToString());
                    if (theNode != null)
                    {
                        theNode = theNode.Clone(true);
                        //INJECT THE CMS PATH
                        WebTrace.Write(this.GetType().ToString(), "Cms Match Value: " + urlMatch.Groups[2].Value);
                        List<CmsPathNode> thePath = CmsPath.GetCmsPath(0, AlwaysConvert.ToInt(urlMatch.Groups[2].Value), CatalogNodeType.Category);
                        WebTrace.Write(this.GetType().ToString(), "thePath.Count: " + thePath.Count.ToString());
                        for (int i = 1; i < thePath.Count; i++)
                        {
                            SiteMapNode newNode = new SiteMapNode(theNode.Provider, thePath[i].NodeId.ToString(), thePath[i].Url, thePath[i].Title, thePath[i].Description);
                            theNode.ReadOnly = false;
                            newNode.ParentNode = theNode.ParentNode;
                            theNode.ParentNode = newNode;
                        }
                    }
                }
            }

            if (theNode == null) {
                theNode = base.FindSiteMapNode(rawUrl);
            }
            return theNode;
        }
    }
}
