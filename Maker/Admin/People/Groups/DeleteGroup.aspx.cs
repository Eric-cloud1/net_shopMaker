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
using MakerShop.Shipping;

public partial class Admin_People_Groups_DeleteGroup : MakerShop.Web.UI.MakerShopAdminPage
{

    int _GroupId = 0;
    MakerShop.Users.Group _Group;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _GroupId = AlwaysConvert.ToInt(Request.QueryString["GroupId"]);
        _Group = GroupDataSource.Load(_GroupId);
        if (_Group == null) Response.Redirect("Default.aspx");
		//you can not delete system Group
        if(_Group.IsReadOnly) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _Group.Name);
        InstructionText.Text = string.Format(InstructionText.Text, _Group.Name);
        BindGroups();
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void DeleteButton_Click(object sender, System.EventArgs e)
    {
        // GET NEW GROUP FOR USERS (IF SELECTED)
        int newGroupId = AlwaysConvert.ToInt(GroupList.SelectedValue);
        GroupDataSource.Delete(_GroupId, newGroupId);
        Response.Redirect("Default.aspx");
    }

    private void BindGroups()
    {
        MakerShop.Users.GroupCollection groups = GroupDataSource.LoadForStore();
        GroupList.DataSource = groups;
        GroupList.DataBind();
    }

}
