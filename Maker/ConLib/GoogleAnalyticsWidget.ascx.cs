using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Stores;
using MakerShop.Utility;
using MakerShop.Products;

public partial class ConLib_GoogleAnalyticsWidget : System.Web.UI.UserControl
{
    private StoreSettingCollection _Settings;
    private Order _Order;
    private int _OrderId = 0;

    protected string GoogleUrchinId
    {
        get
        {
            return _Settings.GoogleUrchinId;
        }
    }

    private bool IsReceiptPage
    {
        get
        {
            return (Request.FilePath.IndexOf("Checkout/Receipt.aspx", StringComparison.InvariantCultureIgnoreCase) >= 0);
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        _Settings = Token.Instance.Store.Settings;
        if (_Settings.EnableGoogleAnalyticsPageTracking && (_Settings.GoogleUrchinId.Length > 0))
        {
            trackingPanel.Visible = true;
            if (_Settings.EnableGoogleAnalyticsEcommerceTracking)
            {
                if (this.IsReceiptPage)
                {
                    _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
                    _Order = OrderDataSource.Load(_OrderId);
                    if (_Order != null && _Order.Items.Count > 0)
                    {
                        RegisterTransactionScript();
                    }
                    else UnRegisterTransactionScript();
                }
                else UnRegisterTransactionScript();
            }
            else UnRegisterTransactionScript();
        }
        else
        {
            trackingPanel.Visible = false;
        }
    }

    private void RegisterTransactionScript()
    {
        string scriptKey = "GoogleAnalyticsTrans:" + this.UniqueID;
        if (!Page.ClientScript.IsStartupScriptRegistered(scriptKey))
        {
            LSDecimal shipping = 0;
            LSDecimal taxes = 0;
            LSDecimal total = 0;
            foreach (OrderItem item in _Order.Items)
            {
                LSDecimal extendedPrice = item.ExtendedPrice;
                switch (item.OrderItemType)
                {
                    case OrderItemType.Shipping:
                    case OrderItemType.Handling:
                        shipping += extendedPrice;
                        break;
                    case OrderItemType.Tax:
                        taxes += extendedPrice;
                        break;
                    default:
                        break;
                }
                total += item.ExtendedPrice;
            }
	        
			string transLine = "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"";
			transLine = string.Format(transLine, _Order.OrderId, "", string.Format("{0:F2}", total), string.Format("{0:F2}", taxes), string.Format("{0:F2}", shipping), _Order.BillToCity, _Order.BillToProvince, _Order.BillToCountryCode);
            
            string scriptBlock =
               @"<script language=""JavaScript"">
				<!--
					pageTracker._addTrans(" + transLine + ");";
            
            string itemLinePattern = "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"";
            foreach (OrderItem item in _Order.Items)
            {
                switch (item.OrderItemType)
                {
                    case OrderItemType.Product:
                        String itemLine = string.Format(itemLinePattern, _Order.OrderId, item.Sku, item.Name, GetItemCategory(item), string.Format("{0:F2}", item.Price), item.Quantity);
                        scriptBlock += @"
							pageTracker._addItem(" + itemLine + ");";
                        break;
                }
            }
            scriptBlock +=
               @"
				  pageTracker._trackTrans();                   
               // -->
               </script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptBlock);
        }
    }

    private void UnRegisterTransactionScript()
    {
        string scriptKey = "GoogleAnalyticsTrans:" + this.UniqueID;
        if (Page.ClientScript.IsStartupScriptRegistered(scriptKey))
        {
            string emptyScriptBlock =
               @"<script language=""JavaScript"">
               <!--
               // -->
               </script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, emptyScriptBlock);
        }
    }

    private string GetAffiliateName(Order orderObj)
    {
        if (orderObj == null || orderObj.Affiliate == null) return "";
        return orderObj.Affiliate.Name;
    }

    private string GetItemCategory(OrderItem item)
    {
        Product product = item.Product;
        string category;

        if (product != null && product.Categories.Count > 0)
        {
            category = CategoryDataSource.GetCategoryName(product.Categories[0]);
            category = StringHelper.CleanupSpecialChars(category);
        }
        else
        {
            category = "None";
        }

        return category;
    }
}
