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
using MakerShop.Shipping.Providers;

public partial class Admin_Orders_Shipments_ViewTrackingNumber : MakerShop.Web.UI.MakerShopAdminPage
{

    int _TrackingNumberId = 0;
    TrackingNumber _TrackingNumber;

    int TrackingNumberId {
        get {
            if (_TrackingNumberId.Equals(0)) {
                _TrackingNumberId = AlwaysConvert.ToInt(Request.QueryString["TrackingNumberId"]);
            }
            return _TrackingNumberId;
        }
    }
    
    TrackingNumber TrackingNumber {
        get {
            if ((_TrackingNumber == null)) {
                _TrackingNumber = new TrackingNumber();
                if (!_TrackingNumber.Load(TrackingNumberId)) {
                    _TrackingNumber = null;
                }
            }
            return _TrackingNumber;
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (TrackingNumber == null)
        {
            Response.Redirect("../Default");
        }
        if (!Page.IsPostBack)
        {
            // ATTEMPT TO GET TRACKING DETAILS
            TrackingNumberData.Text = TrackingNumber.TrackingNumberData;
            if (TrackingNumber.ShipGateway != null)
            {
                MakerShop.Shipping.Providers.IShippingProvider provider = TrackingNumber.ShipGateway.GetProviderInstance();
                MakerShop.Shipping.Providers.TrackingSummary summary = provider.GetTrackingSummary(TrackingNumber);
                if (summary != null)
                {
                    // TRACKING DETAILS FOUND
                    if (summary.TrackingResultType == TrackingResultType.InlineDetails)
                    {
						OrderShipment myShipment = TrackingNumber.OrderShipment;
						Order myOrder = myShipment.Order;
						int myShipmentNumber = (myOrder.Shipments.IndexOf(myShipment.OrderShipmentId) + 1);
						Caption.Text = string.Format(Caption.Text, myOrder.OrderId);
						ShipmentNumber.Text = string.Format(ShipmentNumber.Text, myShipmentNumber, myOrder.Shipments.Count);
						ShippingMethod.Text = myShipment.ShipMethodName;
						PackageCount.Text = summary.PackageCollection.Count.ToString();
						PackageList.DataSource = summary.PackageCollection;
						PackageList.DataBind();

                        DetailsPanel.Visible = true;
                        LinkPanel.Visible = false;
					}else if(summary.TrackingResultType == TrackingResultType.ExternalLink)
					{
                        TrackingLink.NavigateUrl = summary.TrackingLink;
                        TrackingLink.Text = summary.TrackingLink;
                        
                        DetailsPanel.Visible = false;
                        LinkPanel.Visible = true;
                    }
                }
            }
        }
    }

}
