using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Stores;

public partial class ConLib_MyAccountPage : System.Web.UI.UserControl
{
    private int maxOrders = 3;

    protected void Page_Load(object sender, EventArgs e)
    {
        int orderCount = OrderDataSource.CountForUser(Token.Instance.UserId);
        OrderGrid.DataSource = OrderDataSource.LoadForUser(Token.Instance.UserId, maxOrders, 0, "OrderDate DESC");
        OrderGrid.DataBind();
        if (orderCount > maxOrders) ViewAllLink.Visible = true;
        if (!Page.IsPostBack)
        {
            liSubscriptions.Visible = (SubscriptionDataSource.CountForUser(Token.Instance.UserId) > 0);
            liReviews.Visible = (Token.Instance.Store.Settings.ProductReviewEnabled != UserAuthFilter.None);
            liDigitalGoods.Visible = (OrderItemDigitalGoodDataSource.CountForUser(Token.Instance.UserId) > 0);
        }

		if(Token.Instance.Store.Currencies.Count > 1) 
		{
			UserCurrencyAjax.Visible = true;
		}
		else
		{
			UserCurrencyAjax.Visible = false;
		}
    }

    protected string GetShipTo(object dataItem)
    {
        Order order = (Order)dataItem;
        List<string> recipients = new List<string>();
        foreach (OrderShipment shipment in order.Shipments)
        {
            string name = shipment.ShipToFullName;
            if (!recipients.Contains(name)) recipients.Add(name);
        }
        return string.Join(", ", recipients.ToArray());
    }

    protected string GetOrderStatus(object dataItem)
    {
        Order order = (Order)dataItem;
        OrderStatus status = order.OrderStatus;
        if (status == null) return string.Empty;
        return StringHelper.SpaceName(status.DisplayName);
    }

    protected string GetOrderItemName(object dataItem)
    {
        OrderItem orderItem = (OrderItem)dataItem;
        if (orderItem != null)
        {
            if (!string.IsNullOrEmpty(orderItem.VariantName))
            {
                return orderItem.Name + " (" + orderItem.VariantName + ")";
            }
            return orderItem.Name;
        }
        return string.Empty;
    }

    protected void UserCurrency_SelectedIndexChanged(object sender, EventArgs e)
    {
        User user = Token.Instance.User;
        user.UserCurrencyId = AlwaysConvert.ToInt(UserCurrency.SelectedValue);
        user.Save();
    }

    protected void UserCurrency_DataBound(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Store store = Token.Instance.Store;
            UserCurrencyHelpText.Text = string.Format(UserCurrencyHelpText.Text, store.Name, store.BaseCurrency.Name);
            UserCurrencyPanel.Visible = (Token.Instance.Store.Currencies.Count > 1);
            if (UserCurrencyPanel.Visible)
            {
                ListItem item = UserCurrency.Items.FindByValue(Token.Instance.User.UserCurrencyId.ToString());
                if (item != null)
                {
                    UserCurrency.SelectedIndex = UserCurrency.Items.IndexOf(item);
                }
            }
        }
    }
}