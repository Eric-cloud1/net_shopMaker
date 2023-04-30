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
using MakerShop.Users;
using MakerShop.Utility;

public partial class ConLib_MySerialKeyPage : System.Web.UI.UserControl
{
    private int _OrderItemDigitalGoodId;
    private OrderItemDigitalGood _OrderItemDigitalGood;

    protected void Page_Init(object sender, EventArgs e)
    {
        _OrderItemDigitalGoodId = AlwaysConvert.ToInt(Request.QueryString["OrderItemDigitalGoodId"]);
        _OrderItemDigitalGood = OrderItemDigitalGoodDataSource.Load(_OrderItemDigitalGoodId);
        if (_OrderItemDigitalGood == null) Response.Redirect("MyAccount.aspx");
        if ((_OrderItemDigitalGood.OrderItem.Order.UserId != Token.Instance.UserId) && (!Token.Instance.User.IsInRole(Role.OrderAdminRoles))) Response.Redirect(NavigationHelper.GetStoreUrl(this.Page, "Members/MyAccount.aspx"));
        //UPDATE CAPTION
        Caption.Text = String.Format(Caption.Text, _OrderItemDigitalGood.DigitalGood.Name);
        SerialKeyData.Text = _OrderItemDigitalGood.SerialKeyData;
    }
}
