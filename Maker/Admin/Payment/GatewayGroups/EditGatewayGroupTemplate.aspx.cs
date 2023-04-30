using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Reflection;
using System.Collections.Generic;



using MakerShop.Payments;

public partial class Admin_Payment_GatewayGroups_EditGatewayGroupTemplate : System.Web.UI.Page
{
    int paymentGatewayGroupId = 0;
    PaymentGateway paymentGateway = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        paymentGateway = new PaymentGateway();
        gatewayGroupLists.LeftDataSource = PaymentGatewayDataSource.LoadAllGateways(false);
        gatewayGroupLists.LeftDataTextField = "Name";
        gatewayGroupLists.LeftDataValueField = "PaymentGatewayId";
        gatewayGroupLists.DataBind();



        if (!IsPostBack)
        {
            groupLbl.Text = "Group";

            groupdl.DataSource = PaymentGatewayGroupsDataSource.Load();
            groupdl.DataTextField = "PaymentGatewayGroup";
            groupdl.DataValueField = "PaymentGatewayGroupId";
            groupdl.DataBind();

            groupdl.Items.Insert(0, new ListItem("-Select Group-", "0"));

            int.TryParse(groupdl.SelectedValue, out paymentGatewayGroupId);
            if (!string.IsNullOrEmpty(Request.QueryString["PaymentGatewayGroupId"]))
            {
                int.TryParse(Request.QueryString["PaymentGatewayGroupId"], out paymentGatewayGroupId);
                groupdl.SelectedIndex = groupdl.Items.IndexOf(groupdl.Items.FindByValue(paymentGatewayGroupId.ToString()));

            }

            //load all payments that match selected drop down.
            if (paymentGatewayGroupId != 0)
            {
                gatewayGroupLists.RightDataSource = PaymentGatewayDataSource.LoadAllGateways(paymentGatewayGroupId);
                gatewayGroupLists.RightDataTextField = "Name";
                gatewayGroupLists.RightDataValueField = "PaymentGatewayId";
                gatewayGroupLists.DataBind();
            }
        }
    }


    protected void groupdl_select(object sender, EventArgs e)
    {
        int.TryParse(groupdl.SelectedValue, out paymentGatewayGroupId);

        if (paymentGatewayGroupId != 0)
        {
            gatewayGroupLists.RightDataSource = PaymentGatewayDataSource.LoadAllGateways(paymentGatewayGroupId);
            gatewayGroupLists.RightDataTextField = "Name";
            gatewayGroupLists.RightDataValueField = "PaymentGatewayId";
            gatewayGroupLists.DataBind();
        }
    }

    protected void Save_click(object sender, EventArgs e)
    {
        int.TryParse(groupdl.SelectedValue, out paymentGatewayGroupId);

        if ((groupdl.SelectedIndex == 0)||(paymentGatewayGroupId == 0))
        {
            ErrorMessage.Text = "You must select a Group.";
            ErrorMessage.Visible = true;
            return;
        }


        PaymentGatewayGroups pgg = new PaymentGatewayGroups();
        pgg.IsDirty = true;
        pgg.DeleteGroupsPaymentGateways(paymentGatewayGroupId);


        foreach (ListItem item in gatewayGroupLists.RightItems)
        {
            pgg.IsDirty = true;
            pgg.SaveGroupsPaymentGateways(paymentGatewayGroupId, int.Parse(item.Value));
        }



        AddedMessage.Text = string.Format(AddedMessage.Text, groupdl.SelectedItem.Text);
        AddedMessage.Visible = true;
    }

 

}
