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

public partial class Admin_Orders_ViewGiftCertificates : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _OrderId;
    private Order _Order;
	//private int _OrderItemId;
	//private OrderItem _OrderItem;

    protected void Page_Load(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        

		if(_Order==null) {
			Response.Redirect("Default.aspx");
		}

        if (!Page.IsPostBack)
        {
            BindGiftCertificatesGrid();
        }
    }

    protected void BindGiftCertificatesGrid()
    {
        GiftCertificatesGrid.DataSource = GetGiftCertificates();
        GiftCertificatesGrid.DataBind();
    }

    protected GiftCertificateCollection GetGiftCertificates()
    {
		GiftCertificateCollection gcCol;
		gcCol = new GiftCertificateCollection();			
		foreach (OrderItem orderItem in _Order.Items)
		{
			gcCol.AddRange(orderItem.GiftCertificates);
		}	
        return gcCol;
    }

    protected GiftCertificate FindGiftCertificate(int GiftCertificateId)
    {
		GiftCertificate gc = null;					
		foreach (OrderItem orderItem in _Order.Items)
		{
			gc = FindGiftCertificate(orderItem, GiftCertificateId);
			if(gc!=null) break;
		}	
        return gc;
    }

    protected GiftCertificate FindGiftCertificate(OrderItem orderItem, int GiftCertificateId)
    {
        foreach (GiftCertificate gc in orderItem.GiftCertificates)
        {
            if (gc.GiftCertificateId == GiftCertificateId)
            {
                return gc;
            }
        }
        return null;
    }

    protected void GiftCertificatesGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int index = AlwaysConvert.ToInt(e.CommandArgument);
        int GiftCertificateId = AlwaysConvert.ToInt(GiftCertificatesGrid.DataKeys[index].Value);
        GiftCertificate gc = FindGiftCertificate(GiftCertificateId);

		if(gc==null) return;

		IGiftCertKeyProvider provider = new DefaultGiftCertKeyProvider();
		GiftCertificateTransaction trans;

        if (e.CommandName.Equals("Activate"))
        {
				gc.SerialNumber = provider.GenerateGiftCertificateKey();
				trans = new GiftCertificateTransaction();
				trans.Amount = gc.Balance;
				trans.Description = "Gift certificate activated.";
				trans.OrderId = _OrderId;
				trans.TransactionDate = LocaleHelper.LocalNow;
				gc.Transactions.Add(trans);
				gc.Save();
                // Trigger Gift Certificate Activated / Validate Event
                StoreEventEngine.GiftCertificateValidated(gc,trans);
				BindGiftCertificatesGrid();
        }
        else if (e.CommandName.Equals("Deactivate"))
        {
				gc.SerialNumber = "";
				trans = new GiftCertificateTransaction();
				trans.Amount = gc.Balance;
				trans.Description = "Gift certificate deactivated.";
				trans.OrderId = _OrderId;
				trans.TransactionDate = LocaleHelper.LocalNow;
				gc.Transactions.Add(trans);
				gc.Save();
				BindGiftCertificatesGrid();
        }
        else if (e.CommandName.Equals("Edit"))
        {
               //GiftCertificate dg = oigc.GiftCertificate;
               //Response.Redirect("../Products/GiftCertificates/EditGiftCertificate.aspx?ProductId=" + dg.ProductId.ToString() + "&GiftCertificateId=" + dg.GiftCertificateId.ToString());
        }
    }

    protected void GiftCertificatesGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {        
        int GiftCertificateId = (int)GiftCertificatesGrid.DataKeys[GiftCertificatesGrid.Rows[e.RowIndex].DataItemIndex].Value;
		DeleteGiftCertificate(GiftCertificateId);
		BindGiftCertificatesGrid();
    }

    protected void DeleteGiftCertificate(int GiftCertificateId)
    {
		GiftCertificate gc = null;					
		foreach (OrderItem orderItem in _Order.Items)
		{
			gc = FindGiftCertificate(orderItem, GiftCertificateId);
			if(gc!=null) {
				orderItem.GiftCertificates.Remove(gc);
				gc.Delete();				
				return;
			}
		}
    }

    protected bool HasSerialKey(object obj)
    {
		GiftCertificate oigc = (GiftCertificate)obj;
        return oigc.SerialNumber != null && oigc.SerialNumber.Length > 0;
	}

    protected OrderItem FindOrderItem(int orderItemId)
    {
		foreach (OrderItem orderItem in _Order.Items)
		{
			if(orderItem.OrderItemId == orderItemId) return orderItem;
		}
		return null;
	}

    protected string GetEditUrl(object obj)
    {
		GiftCertificate gc = (GiftCertificate)obj;
		return string.Format("~/Admin/Payment/EditGiftCertificate.aspx?GiftCertificateId={0}&OrderId={1}",gc.GiftCertificateId, _OrderId);        
	}


}
