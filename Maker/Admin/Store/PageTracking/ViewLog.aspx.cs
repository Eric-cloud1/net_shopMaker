using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Reporting;
using System.Collections.Generic;

public partial class Admin_Store_PageTracking_ViewLog : MakerShop.Web.UI.MakerShopAdminPage
{


    protected void PageViewsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            PageView pageView = (PageView)e.Row.DataItem;
            User user = pageView.User;
            if (user != null)
            {
                PlaceHolder phUser = (PlaceHolder)e.Row.FindControl("phUser");
                if (phUser != null)
                {
                    HyperLink userLink = new HyperLink();
                    userLink.Text = user.UserName;                    
                    userLink.NavigateUrl = "~/Admin/People/Users/EditUser.aspx?UserId=" + user.UserId.ToString();
                    phUser.Controls.Add(new LiteralControl("<div style=\"max-width:200px;height:40px;overflow:auto;\">") );
                    phUser.Controls.Add(userLink );
                    phUser.Controls.Add(new LiteralControl("</div>"));
                }
            }
            ICatalogable catalogNode = pageView.CatalogNode;
            if (catalogNode != null)
            {
                PlaceHolder phCatalogNode = (PlaceHolder)e.Row.FindControl("phCatalogNode");
                if (phCatalogNode != null)
                {
                    HyperLink catalogLink = new HyperLink();
                    catalogLink.Text = catalogNode.Name;
                    catalogLink.NavigateUrl = catalogNode.NavigateUrl;
                    phCatalogNode.Controls.Add(catalogLink);
                }
            }
        }
    }

    protected string GetQueryString(object dataItem)
    {
        string uriQuery = (string)dataItem;
        if (string.IsNullOrEmpty(uriQuery)) return string.Empty;
        if (!uriQuery.StartsWith("?")) return "?" + uriQuery;
        return uriQuery;
    }

}
