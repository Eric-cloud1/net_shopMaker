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
using MakerShop.Payments;

public partial class Admin_Reports_GiftCertTransactions : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _GiftCertificateId;
    private GiftCertificate _GiftCertificate;

    protected void Page_Init(object sender, EventArgs e)
    {
        _GiftCertificateId = AlwaysConvert.ToInt(Request.QueryString["GiftCertificateId"]);
        _GiftCertificate = GiftCertificateDataSource.Load(_GiftCertificateId);
        if (_GiftCertificate == null) Response.Redirect("Default.aspx");
        
		if (!Page.IsPostBack)
        {            
            BindTransactions();
			Caption.Text = string.Format(Caption.Text,_GiftCertificate.Name);
        }
    }
    
    protected void BindTransactions()
    {
        TransactionGrid.DataSource = _GiftCertificate.Transactions;
        TransactionGrid.DataBind();        
    }

    protected string GetOrderNumber(object obj)
    {
        GiftCertificateTransaction trans = obj as GiftCertificateTransaction;
        if (trans == null) return "";
        if (trans.Order == null) return "";
        return trans.Order.OrderNumber.ToString();
    }

    protected string GetOrderLink(object obj)
    {
        GiftCertificateTransaction trans = obj as GiftCertificateTransaction;
        if (trans == null) return "";
        if (trans.Order == null) return "";
        return "~/Admin/Orders/ViewOrder.aspx?OrderNumber=" + trans.Order.OrderNumber.ToString() + "&OrderId=" + trans.Order.OrderId.ToString();
    }

    protected bool HasOrder(object obj)
    {
		GiftCertificateTransaction trans = obj as GiftCertificateTransaction;
		if (trans == null) return false;
		if(trans.Order == null) return false;
		return trans.Order.OrderId > 0;
	}



}
