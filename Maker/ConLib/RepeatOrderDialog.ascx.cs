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
using MakerShop.Products;
using MakerShop.Utility;

public partial class ConLib_RepeatOrderDialog : System.Web.UI.UserControl
{
    private Order _Order;
    private int _OrderId = 0;
    private string _Caption = "Repeat Order";

    [Personalizable(), WebBrowsable()]
    public string Caption
    {
        get { return _Caption; }
        set { _Caption = value; }
    }

    private bool IsRepeatable
    {
        get
        {
            int availableCount = 0;
            if (_Order != null)
            {
                foreach (OrderItem item in _Order.Items)
                {
                    if ((item.OrderItemType == OrderItemType.Product) && (!item.IsChildItem))
                    {
                        Product product = item.Product;
                        if ((product != null) && (product.Visibility != MakerShop.Catalog.CatalogVisibility.Private))
                        {
                            availableCount++;
                        }
                    }
                }
            }
            return (availableCount > 0);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        if (this.IsRepeatable)
        {
            phRepeatable.Visible = !Token.Instance.Store.Settings.ProductPurchasingDisabled;
            ReorderButton.NavigateUrl = "~/Reorder.ashx?o=" + _OrderId.ToString();
        }
        else phRepeatable.Visible = false;
    }
}