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

public partial class Admin_Orders_Edit_EditOrderItems : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _OrderId;
    private Order _Order;

    protected void Page_Load(object sender, EventArgs e)
    {
      //  if (true)
      //  {
       //     string s;
      //      Table list = ((Admin_UserControls_OrderEditItemDetail)OrderItemGrid.Rows[1].Controls[1].Controls[1]).ItemTable;

       //     for (int i = 0; i < list.Rows.Count; i++)
      //      {
      //          s = ((TextBox)list.Rows[i].Controls[1].Controls[0]).Text;
      //      }
     //   }
    }
    protected void Page_Init(object sender, EventArgs e)
    {
        
     
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        if (_Order == null) Response.Redirect("Default.aspx");
        string suffix = "?OrderNumber=" + _Order.OrderNumber + "&OrderId=" + _OrderId.ToString();
        AddProductLink.NavigateUrl += suffix;
        AddOtherItemLink.NavigateUrl += suffix;
        OrderItemGrid.Columns[4].Visible = TaxHelper.ShowTaxColumn;
        OrderItemGrid.Columns[4].HeaderText = TaxHelper.TaxColumnHeader;
        RecalculateTaxesButton.Visible = TaxHelper.IsATaxProviderEnabled();
    }
    
    protected void OrderItemGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int orderItemId = (int)OrderItemGrid.DataKeys[e.RowIndex].Value;
        int index = _Order.Items.IndexOf(orderItemId);
        if (index > -1)
        {
            OrderItem orderItem = _Order.Items[index];
            orderItem.Price = AlwaysConvert.ToDecimal(e.NewValues["Price"]);
            orderItem.Quantity = AlwaysConvert.ToInt16(e.NewValues["Quantity"]);
            if (OrderItemGrid.Columns[4].Visible)
                orderItem.TaxRate = AlwaysConvert.ToDecimal(e.NewValues["TaxRate"]);

            for (int j = 0; j < orderItem.Inputs.Count; j++ )
            {
                Control detail = OrderItemGrid.Rows[index].Controls[1].Controls[1];
                for (int i = 6 ; i < detail.Controls.Count; i++)
                {
                    i = i + (4 * j) ;

                     orderItem.Inputs[j].InputValue = ((TextBox)detail.Controls[i]).Text;
                       
                   break;

                }
                orderItem.Inputs.Save();
            }

         
         
			// We should not allow positive values for Discount or Credit. We should not
            // allow negative values for Charge. bug 4969
			switch (orderItem.OrderItemTypeId)
            {                
                case ((short)OrderItemType.Discount):
                case ((short)OrderItemType.Credit):
                    if (orderItem.Price > 0) orderItem.Price = Decimal.Negate((Decimal)orderItem.Price);
                    break;
                
                case ((short)OrderItemType.Charge):
                    if (orderItem.Price < 0) orderItem.Price = Decimal.Negate((Decimal)orderItem.Price);
                    break;
				default:
                    break;
            }

            //UPDATE THE CALCULATED SUMMARY VALUES OF ORDER
            _Order.Save();
        }
        OrderItemGrid.EditIndex = -1;
        e.Cancel = true;
    }

    protected bool IsGiftCert(object dataItem)
    {
        OrderItem orderItem = dataItem as OrderItem;
        if(orderItem == null) return false;		
        return (orderItem.GiftCertificates.Count > 0);
    }

    protected void OrderItemGrid_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
        //UPDATE THE CALCULATED SUMMARY VALUES OF ORDER
        _Order.Save();
    }

    protected void RecalculateTaxesButton_OnClick(object sender, EventArgs e)
    {
        // RECALCULATE TAXES FOR THE ORDER
        TaxCalculator.Recalculate(_Order);
        OrderItemGrid.DataBind();
    }
}
