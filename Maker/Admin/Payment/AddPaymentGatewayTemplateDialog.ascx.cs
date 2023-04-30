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
using MakerShop.Payments;
using System.Collections.Generic;
using MakerShop.Common;
using MakerShop.Utility;
using MakerShop.Users;

public partial class Admin_Payment_AddPaymentGatewayTemplateDialog : System.Web.UI.UserControl
{
    //DEFINE AN EVENT TO TRIGGER WHEN AN ITEM IS ADDED 
    public event PersistentItemEventHandler ItemAdded;
    private const bool ShowIntlPaymentMethods = true;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
          
         
        }
    }

    protected void AddButton_Click(object sender, System.EventArgs e)
    {
        PaymentGatewayTemplate pgt = new PaymentGatewayTemplate();
        pgt.Name = Name.Text;
        pgt.Save();  

        //UPDATE THE ADD MESSAGE
        AddedMessage.Text = string.Format(AddedMessage.Text, pgt.Name);
        AddedMessage.Visible = true;

        //RESET THE ADD FORM
        Name.Text = string.Empty;
        
        //TRIGER ANY EVENT ATTACHED TO THE UPDATE
        if (ItemAdded != null) ItemAdded(this, new PersistentItemEventArgs(pgt.PaymentGatewayTemplateId, pgt.Name));
    }

}
