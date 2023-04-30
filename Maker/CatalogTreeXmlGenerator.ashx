<%@ WebHandler Language="C#" Class="CatalogTreeXmlGenerator" %>

using System;
using System.Web;
using MakerShop.Catalog;
using MakerShop.Products;
using ComponentArt.Web.UI;

public class CatalogTreeXmlGenerator : IHttpHandler
{
    int _CategoryId = 0;
    Category _Category = null;    
    bool _ShowAllNodes = true;
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/xml";
        _CategoryId = MakerShop.Utility.AlwaysConvert.ToInt(context.Request.QueryString["CategoryId"]);
        _Category = CategoryDataSource.Load(_CategoryId);                
        _ShowAllNodes = MakerShop.Utility.AlwaysConvert.ToBool(context.Request.QueryString["ShowAllNodes"], true);
        
        context.Response.Write(GetCategoryNodes(_CategoryId, context));     
        context.Response.End();   
    }
 
    public bool IsReusable{
        get {
            return true;
        }
    }

    // Builds a TreeView level containing the list of sub-categories of the given category    
    private String GetCategoryNodes(int rootCategoryId, HttpContext context)
    {
        TreeView TreeView1 = new ComponentArt.Web.UI.TreeView();
        TreeViewNodeCollection nodes = TreeView1.Nodes;

        CatalogNodeCollection catalogNodes = null;
        ComponentArt.Web.UI.TreeViewNode node;
        if (!_ShowAllNodes)
        {
            catalogNodes = CatalogNodeDataSource.LoadForCriteria("CategoryId = " + rootCategoryId + " And CatalogNodeTypeId = " + (int)CatalogNodeType.Category);
        }
        else
        {
            catalogNodes = CatalogNodeDataSource.LoadForCategory(rootCategoryId);
        }        

        foreach (CatalogNode catalogNode in catalogNodes)
        {            
            // SKIP NON-PUBLIC NODES
            if (catalogNode.Visibility != CatalogVisibility.Public) continue;

            switch (catalogNode.CatalogNodeType)
            {
                case CatalogNodeType.Category:
                    Category category = ((Category)catalogNode.ChildObject);
                    if (category != null)
                    {
                        node = new ComponentArt.Web.UI.TreeViewNode();
                        node.ID = "Category_" + category.CategoryId.ToString();
                        node.Text = category.Name;
                        node.NavigateUrl = category.NavigateUrl;
                        node.ImageUrl = "category.gif";
                        node.Expanded = false;
                        String callbackUrl = context.Request.Url.AbsoluteUri.ToString();
                        callbackUrl = callbackUrl.Substring(0,callbackUrl.IndexOf('?'));
                        node.ContentCallbackUrl = callbackUrl + "?CategoryId=" + category.CategoryId.ToString() +  "&ShowAllNodes=" + _ShowAllNodes;                        
                        nodes.Add(node);
                    }
                    break;
                case CatalogNodeType.Product:
                    Product product = ((Product)catalogNode.ChildObject);
                    if (product != null)
                    {
                        node = new ComponentArt.Web.UI.TreeViewNode();
                        node.ID = "Product_" + product.ProductId.ToString();
                        node.Text = product.Name;
                        node.NavigateUrl = product.NavigateUrl;
                        node.ImageUrl = "product.gif";
                        nodes.Add(node);
                    }
                    break;
                case CatalogNodeType.Webpage:
                    Webpage webpage = ((Webpage)catalogNode.ChildObject);
                    if (webpage != null)
                    {
                        node = new ComponentArt.Web.UI.TreeViewNode();
                        node.ID = "Webpage_" + webpage.WebpageId.ToString();
                        node.Text = webpage.Name;
                        node.NavigateUrl = webpage.NavigateUrl;
                        node.ImageUrl = "webpage.gif";
                        nodes.Add(node);
                    }
                    break;

                case CatalogNodeType.Link:
                    Link link = ((Link)catalogNode.ChildObject);
                    if (link != null)
                    {
                        node = new ComponentArt.Web.UI.TreeViewNode();
                        node.ID = "Link_" + link.LinkId.ToString();
                        node.Text = link.Name;
                        node.NavigateUrl = link.NavigateUrl;
                        node.ImageUrl = "link.gif";
                        nodes.Add(node);
                    }
                    break;
                default: break;
            }
        }
        return TreeView1.GetXml();
    }
}