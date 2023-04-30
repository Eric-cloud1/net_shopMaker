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
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Shipping.Providers;
using MakerShop.Utility;

public partial class ConLib_MyTrackingInfoPage : System.Web.UI.UserControl
{
    int _TrackingNumberId = 0;
    TrackingNumber _TrackingNumber;

    int TrackingNumberId
    {
        get
        {
            if (_TrackingNumberId.Equals(0))
            {
                _TrackingNumberId = AlwaysConvert.ToInt(Request.QueryString["TrackingNumberId"]);
            }
            return _TrackingNumberId;
        }
    }

    TrackingNumber TrackingNumber
    {
        get
        {
            if ((_TrackingNumber == null))
            {
                _TrackingNumber = new TrackingNumber();
                if (!_TrackingNumber.Load(TrackingNumberId))
                {
                    _TrackingNumber = null;
                }
            }
            return _TrackingNumber;
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (TrackingNumber == null) Response.Redirect("MyAccount.aspx");
        if ((TrackingNumber.OrderShipment.Order.UserId != Token.Instance.UserId) && (!IsInAdminRole()))
        {
            Response.Redirect("MyAccount.aspx");
        }
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, TrackingNumber.OrderShipment.Order.OrderId);
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
                        ShipmentNumber.Text = string.Format(ShipmentNumber.Text, myShipmentNumber, myOrder.Shipments.Count);
                        ShippingMethod.Text = myShipment.ShipMethodName;
                        PackageCount.Text = summary.PackageCollection.Count.ToString();
                        PackageList.DataSource = summary.PackageCollection;
                        PackageList.DataBind();

                        DetailsPanel.Visible = true;
                        LinkPanel.Visible = false;
                    }
                    else if (summary.TrackingResultType == TrackingResultType.ExternalLink)
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
    private bool IsInAdminRole()
    {
        return Token.Instance.User.IsInRole("Admin")
               || Token.Instance.User.IsInRole("System")
               || Token.Instance.User.IsInRole("Jr. Admin")
               || Token.Instance.User.IsInRole("Manage Orders");
    }
}
