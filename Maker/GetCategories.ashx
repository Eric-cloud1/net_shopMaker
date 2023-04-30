<%@ WebHandler Language="C#" Class="GetCategories" %>

using System;
using System.Web;
using MakerShop.Catalog;
using MakerShop.Utility;

public class GetCategories : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        context.Response.Clear();
        context.Response.ContentType = "text/xml";
        ComponentArt.Web.UI.TreeView CategoryTree = new ComponentArt.Web.UI.TreeView();
        int categoryId = PageHelper.GetCategoryId();
        CategoryCollection children = CategoryDataSource.LoadForParent(categoryId, true);
        foreach (Category child in children)
        {
            ComponentArt.Web.UI.TreeViewNode newNode = new ComponentArt.Web.UI.TreeViewNode();
            newNode.Text = child.Name;
            newNode.ID = child.CategoryId.ToString();
            if (CategoryDataSource.CountForParent(child.CategoryId) > 0)
            {
                newNode.ContentCallbackUrl = CategoryTree.ResolveClientUrl("~/GetCategories.ashx?CategoryId=" + newNode.ID);
            }
            else
            {
                newNode.Expanded = true;
            }
            newNode.NavigateUrl = child.NavigateUrl;
            CategoryTree.Nodes.Add(newNode);
        }
        context.Response.Write(CategoryTree.GetXml());
        context.Response.End();
    }
    
    public bool IsReusable {
        get {
            return true;
        }
    }

}