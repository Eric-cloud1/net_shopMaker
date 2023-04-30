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
using MakerShop.Common;
using MakerShop.Marketing;
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Catalog;
using MakerShop.Products;
using System.Collections.Generic;

public partial class ConLib_CategoryBreadCrumbs : System.Web.UI.UserControl
{
    int _CategoryId = 0;
    bool _HideLastNode = false;
    string _Separator = "&nbsp;>&nbsp;";

    public string Separator
    {
        get { return _Separator; }
    }

    public int CategoryId
    {
        get { return _CategoryId; }
        set { _CategoryId = value; }
    }

    public bool HideLastNode
    {
        get { return _HideLastNode; }
        set { _HideLastNode = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.CategoryId = PageHelper.GetCategoryId(true);
    }

    protected void Page_PreRender(object sender, System.EventArgs e)
    {
        HomeLink.NavigateUrl = NavigationHelper.GetHomeUrl();
        if (this.CategoryId != 0)
        {
            List<CatalogPathNode> path = CatalogDataSource.GetPath(CategoryId, false);
            BreadCrumbsRepeater.DataSource = path;
            BreadCrumbsRepeater.DataBind();
            if ((HideLastNode) && (BreadCrumbsRepeater.Controls.Count > 0))
            {
                BreadCrumbsRepeater.Controls[(BreadCrumbsRepeater.Controls.Count - 1)].Visible = false;
            }
        }
        else BreadCrumbsRepeater.Visible = false;
    }
}