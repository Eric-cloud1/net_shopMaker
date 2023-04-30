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

public partial class Admin_Orders_Shipments_FindProduct : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _OrderShipmentId;
    private OrderShipment _OrderShipment;
    public void Page_Init(object sender, EventArgs e)
    {
        _OrderShipmentId = AlwaysConvert.ToInt(Request.QueryString["OrderShipmentId"]);
        _OrderShipment = OrderShipmentDataSource.Load(_OrderShipmentId);
        if (_OrderShipment == null) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _OrderShipment.ShipmentNumber, _OrderShipment.OrderId);
    }       

}
