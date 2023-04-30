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
using AjaxControlToolkit;
using ComponentArt.Web.UI;

public partial class Admin_Store_OrderStatuses_SortStatuses : MakerShop.Web.UI.MakerShopAdminPage
{

    private OrderStatusCollection _OrderStatuses;

    protected void Page_Load(object sender, EventArgs e)
    {
        _OrderStatuses = OrderStatusDataSource.LoadForStore();
        if (!Page.IsPostBack && !ReorderCallback.IsCallback)
        {
            BindOrderStatusList();
        }
    }
        
     protected void ReorderMethod(int oldIndex, int newIndex)
    {
        if ((oldIndex < 0) || (oldIndex >= _OrderStatuses.Count)) return;
        if ((newIndex < 0) || (newIndex >= _OrderStatuses.Count)) return;
        if (oldIndex == newIndex) return;
        //MAKE SURE ITEMS ARE IN CORRECT ORDER
        for (short i = 0; i < _OrderStatuses.Count; i++)
            _OrderStatuses[i].OrderBy = (short)(i + 1);
        //LOCATE THE DESIRED ITEM
        OrderStatus temp = _OrderStatuses[oldIndex];
        _OrderStatuses.RemoveAt(oldIndex);
        if (newIndex < _OrderStatuses.Count)
            _OrderStatuses.Insert(newIndex, temp);
        else
            _OrderStatuses.Add(temp);
        //MAKE SURE ITEMS ARE IN CORRECT ORDER
        for (short i = 0; i < _OrderStatuses.Count; i++)
            _OrderStatuses[i].OrderBy = (short)(i + 1);
        _OrderStatuses.Save();
    }

    protected void OrderStatusList_ItemReordering(object sender, ReorderListItemReorderEventArgs e)
    {
        //REORDER HERE
        ReorderMethod(e.OldIndex, e.NewIndex);
        BindOrderStatusList();
        //e.Cancel = true;
    }

    protected void BindOrderStatusList()
    {
        OrderStatusList.DataSource = _OrderStatuses;
        OrderStatusList.DataBind();
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void ReorderCallback_Callback(object sender, CallBackEventArgs e)
    {
        try
        {
            string[] moveargs = e.Parameter.Split(":".ToCharArray());
            int source = AlwaysConvert.ToInt(moveargs[0]);
            int target = AlwaysConvert.ToInt(moveargs[1]);
            ReorderMethod(source, target);
        }
        catch { }
    }


}
