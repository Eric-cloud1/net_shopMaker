using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using MakerShop.Catalog;
using MakerShop.Data;
using MakerShop.DigitalDelivery;
using MakerShop.Marketing;
using MakerShop.Messaging;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Payments.Providers;
using MakerShop.Products;
using MakerShop.Reporting;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Taxes;
using MakerShop.Taxes.Providers;
using MakerShop.Users;
using MakerShop.Utility;

public partial class Admin_Help_ErrorLog : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void ErrorMessageGrid_DataBound(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in ErrorMessageGrid.Rows)
        {
            CheckBox cb = (CheckBox)gvr.FindControl("Selected");
            ScriptManager.RegisterArrayDeclaration(ErrorMessageGrid, "CheckBoxIDs", String.Concat("'", cb.ClientID, "'"));
        }
    }

    private List<int> GetSelectedErrorMessageIds()
    {
        List<int> selectedErrorMessages = new List<int>();
        foreach (GridViewRow row in ErrorMessageGrid.Rows)
        {
            CheckBox selected = (CheckBox)PageHelper.RecursiveFindControl(row, "Selected");
            if ((selected != null) && selected.Checked)
            {
                selectedErrorMessages.Add((int)ErrorMessageGrid.DataKeys[row.DataItemIndex].Values[0]);
            }
        }
        return selectedErrorMessages;
    }

    protected void DeleteSelectedButton_Click(object sender, EventArgs e)
    {
        List<int> errorMessageIds = GetSelectedErrorMessageIds();
        foreach(int id in errorMessageIds)
        {
            ErrorMessageDataSource.Delete(id);
            ErrorMessageGrid.DataBind();
        }
    }

    protected void DeleteAllButton_Click(object sender, EventArgs e)
    {
        ErrorMessageDataSource.DeleteForStore();
        ErrorMessageGrid.DataBind();
    }

	protected void Page_PreRender(object sender, EventArgs e)
    {
        if (ErrorMessageGrid.Rows.Count > 0)
        {
            ButtonsPlaceHolder.Visible = true;
        }
        else
        {
            ButtonsPlaceHolder.Visible = false;
        }
	}
}
