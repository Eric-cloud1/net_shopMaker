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
using MakerShop.Payments;

public partial class Admin_Payment_Methods : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void PaymentMethodGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        int paymentMethodId = (int)PaymentMethodGrid.DataKeys[e.NewEditIndex].Value;
        PaymentMethod paymentMethod = PaymentMethodDataSource.Load(paymentMethodId);
        if (paymentMethod != null)
        {
            AddPanel.Visible = false;
            EditPanel.Visible = true;
            EditCaption.Text = string.Format(EditCaption.Text, paymentMethod.Name);
            ASP.admin_payment_editpaymentmethoddialog_ascx editDialog = EditPanel.FindControl("EditPaymentMethodDialog1") as ASP.admin_payment_editpaymentmethoddialog_ascx;
            if (editDialog != null) editDialog.LoadDialog(paymentMethodId);
            UpdatePanel1.Update();
        }
    }

    protected void PaymentMethodGrid_RowCancelingEdit(object sender, EventArgs e)
    {
        AddPanel.Visible = true;
        EditPanel.Visible = false;
        UpdatePanel1.Update();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AddPaymentMethodDialog1.ItemAdded += new PersistentItemEventHandler(AddPaymentMethodDialog1_ItemAdded);
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        string sqlCriteria = "PaymentInstrumentId<>" + (short)PaymentInstrument.GiftCertificate
            + " AND PaymentInstrumentId<>" + (short)PaymentInstrument.GoogleCheckout
        +" AND IsActive=1 ";
        PaymentMethodDs.SelectParameters[0].DefaultValue = sqlCriteria;
    }
    
    void AddPaymentMethodDialog1_ItemAdded(object sender, PersistentItemEventArgs e)
    {
        RebindPage();
    }

    protected void EditPaymentMethodDialog1_ItemUpdated(object sender, PersistentItemEventArgs e)
    {
        DoneEditing();
    }

    protected void EditPaymentMethodDialog1_Cancelled(object sender, EventArgs e)
    {
        DoneEditing();
    }

    protected void DoneEditing()
    {
        AddPanel.Visible = true;
        EditPanel.Visible = false;
        PaymentMethodGrid.EditIndex = -1;
        RebindPage();
    }

    protected void RebindPage()
    {
        PaymentMethodGrid.DataBind();
        UpdatePanel1.Update();
    }

    protected void PaymentMethodGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "MoveUp")
        {
            PaymentMethodCollection paymentMethods = PaymentMethodDataSource.LoadForStore();
            int itemIndex = AlwaysConvert.ToInt(e.CommandArgument);
            if ((itemIndex < 1) || (itemIndex > paymentMethods.Count - 1)) return;
            PaymentMethod selectedItem = paymentMethods[itemIndex];
            PaymentMethod swapItem = paymentMethods[itemIndex - 1];
            paymentMethods.RemoveAt(itemIndex - 1);
            paymentMethods.Insert(itemIndex, swapItem);
            for (int i = 0; i < paymentMethods.Count; i++)
            {
                paymentMethods[i].OrderBy = (short)i;
            }
            paymentMethods.Save();
        }
        else if (e.CommandName == "MoveDown")
        {
            PaymentMethodCollection paymentMethods = PaymentMethodDataSource.LoadForStore();
            int itemIndex = AlwaysConvert.ToInt(e.CommandArgument);
            if ((itemIndex > paymentMethods.Count - 2) || (itemIndex < 0)) return;
            PaymentMethod selectedItem = paymentMethods[itemIndex];
            PaymentMethod swapItem = paymentMethods[itemIndex + 1];
            paymentMethods.RemoveAt(itemIndex + 1);
            paymentMethods.Insert(itemIndex, swapItem);
            for (int i = 0; i < paymentMethods.Count; i++)
            {
                paymentMethods[i].OrderBy = (short)i;
            }
            paymentMethods.Save();
        }
        PaymentMethodGrid.DataBind();
    }
    

}
