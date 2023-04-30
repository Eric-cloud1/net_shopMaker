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

public partial class Admin_People_Groups_Default : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Page_Init(object sender, EventArgs e)
    {
        AddGroupDialog1.ItemAdded += new PersistentItemEventHandler(AddGroupDialog1_ItemAdded);
        EditGroupDialog1.ItemUpdated += new PersistentItemEventHandler(EditGroupDialog1_ItemUpdated);
        User user = Token.Instance.User;
        IsSuperUser = user.IsInRole("System");
        IsSecurityAdmin = IsSuperUser || (user.IsInRole("Admin"));
    }

    private void AddGroupDialog1_ItemAdded(object sender, PersistentItemEventArgs e)
    {
        GroupGrid.DataBind();
        GroupAjax.Update();
    }

    private void EditGroupDialog1_ItemUpdated(object sender, PersistentItemEventArgs e)
    {
        GroupGrid.EditIndex = -1;
        GroupGrid.DataBind();        
        AddPanel.Visible = true;
        EditPanel.Visible = false;
        AddEditAjax.Update();
        GroupAjax.Update();
    }

    protected void GroupGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        int groupId = (int)GroupGrid.DataKeys[e.NewEditIndex].Value;
        MakerShop.Users.Group group = GroupDataSource.Load(groupId);
        if (group != null)
        {
            AddPanel.Visible = false;
            EditPanel.Visible = true;
            EditCaption.Text = string.Format(EditCaption.Text, group.Name);
            ASP.admin_people_groups_editgroupdialog_ascx editDialog = EditPanel.FindControl("EditGroupDialog1") as ASP.admin_people_groups_editgroupdialog_ascx;
            if (editDialog != null) editDialog.GroupId = groupId;
            AddEditAjax.Update();
        }
    }

    protected void GroupGrid_RowCancelingEdit(object sender, EventArgs e)
    {
        AddPanel.Visible = true;
        EditPanel.Visible = false;
        AddEditAjax.Update();
    }

    protected string GetRoles(object dataItem)
    {
        MakerShop.Users.Group group = (MakerShop.Users.Group)dataItem;
        List<string> roles = new List<string>();
        foreach (GroupRole groupRole in group.GroupRoles)
        {
            roles.Add(groupRole.Role.Name);
        }
        return string.Join(", ", roles.ToArray());
    }
    
    protected int CountUsers(object dataItem)
    {
        MakerShop.Users.Group group = (MakerShop.Users.Group)dataItem;
        return UserDataSource.CountForGroup(group.GroupId);
    }

    protected bool IsSecurityAdmin = false;
    protected bool IsSuperUser = false;

    protected bool IsEditableGroup(object dataItem)
    {
        MakerShop.Users.Group group = (MakerShop.Users.Group)dataItem;
        if(group.IsInRole("System")){
            if(Token.Instance.User.IsSystemAdmin)
            {
                return (IsSecurityAdmin && (!group.IsReadOnly));
            }else{
                return false;
            }
        }
        else return (IsSecurityAdmin && (!group.IsReadOnly));
    }

    protected bool ShowDeleteButton(object dataItem)
    {
        MakerShop.Users.Group g = (MakerShop.Users.Group)dataItem;
        if (!IsEditableGroup(dataItem)) return false;
        return (UserDataSource.CountForGroup(g.GroupId) <= 0);
    }

    protected bool ShowDeleteLink(object dataItem)
    {
        MakerShop.Users.Group g = (MakerShop.Users.Group)dataItem;
        if (!IsEditableGroup(dataItem)) return false;
        return (UserDataSource.CountForGroup(g.GroupId) > 0);
    }

}
