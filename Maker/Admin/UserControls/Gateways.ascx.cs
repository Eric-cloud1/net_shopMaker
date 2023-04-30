using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.ComponentModel;

public partial class Admin_UserControls_Gateways : System.Web.UI.UserControl
{
    public enum showStyle
    {
        Gateway,
        PaymentGateway,
        Gateway_PaymentGateway

    }
    private showStyle _showStyle = showStyle.Gateway_PaymentGateway;
    [Browsable(true), DefaultValue(showStyle.Gateway_PaymentGateway)]
    public showStyle ShowStyle
    {
        get
        {
            return _showStyle;
        }
        set { _showStyle = value; }
    }


    [Browsable(true), DefaultValue(false)]
    public bool ShowInActive
    {
        get
        {
            return cbShowInActive.Checked;
        }
        set { cbShowInActive.Checked = value; }
    }
    public string Gateway
    {
        get
        {
            return GatewayNames.SelectedItem.Text;
        }
    }
    public int PaymentGatewayId
    {
        get
        {
            int i;
            if (int.TryParse(PaymentGateways.SelectedValue, out i))
                return i;
            return -1;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            switch (ShowStyle)
            {
                case showStyle.Gateway:
                    rowPaymentGateways.Visible = false;
                    bindGateways();
                    break;
                case showStyle.PaymentGateway:
                    rowGateways.Visible = false;
                    bindPaymentGateways();
                    break;
                case showStyle.Gateway_PaymentGateway:
                    bindGateways();
                    break;
                default:         
                    break;
            }

        }
    }
    private void bindGateways()
    {
        GatewayNames.DataSource = MakerShop.Payments.PaymentGateway.GetGateways();

        GatewayNames.DataTextField = "Name";
        GatewayNames.DataValueField = "Name";

        GatewayNames.DataBind();

        GatewayNames.Items.Insert(0, new ListItem("-- All --", "-1"));
        GatewayNames.SelectedIndex = 0;

        bindPaymentGateways();
    }
    private void bindPaymentGateways()
    {
        MakerShop.Payments.PaymentGatewayCollection c =GetPaymentGateways();
        if ((c == null) || (c.Count == 0))
        {
            PaymentGateways.Items.Clear();
            PaymentGateways.Enabled = false;
            PaymentGateways.Items.Add("-- NONE --");
        }
        else
        {

            PaymentGateways.DataSource = c;
            PaymentGateways.DataTextField = "Name";
            PaymentGateways.DataValueField = "PaymentGatewayId";
            PaymentGateways.DataBind();
            PaymentGateways.Enabled = true;
        }
    }

    private MakerShop.Payments.PaymentGatewayCollection GetPaymentGateways()
    {
        string sqlCriteria = string.Empty;

        sqlCriteria = " Name<>'Gift Certificate Payment Provider' AND ClassId<>'" + MakerShop.Utility.StringHelper.SafeSqlString(MakerShop.Utility.Misc.GetClassId(typeof(MakerShop.Payments.Providers.GiftCertificatePaymentProvider))) + "'";
        if (!cbShowInActive.Checked) sqlCriteria += " AND IsActive=1 ";

        if (GatewayNames.SelectedIndex != 0)
        {
            sqlCriteria = " Name<>'Gift Certificate Payment Provider' AND name like  '%" + GatewayNames.SelectedValue + "%'";
            if (!cbShowInActive.Checked) sqlCriteria += " AND IsActive=1 ";
        }
        sqlCriteria += " ORDER BY Name";
        return MakerShop.Payments.PaymentGatewayDataSource.LoadForCriteria(sqlCriteria);
    }

    protected void GatewayNames_SelectedIndexChanged(object sender, EventArgs e)
    {
        bindPaymentGateways();
    }
    
    protected void cbShowInActive_CheckedChanged(object sender, EventArgs e)
    {
        bindPaymentGateways();
    }
}
