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
using MakerShop.DigitalDelivery;
using MakerShop.Orders;

public partial class ConLib_MyDigitalGoodsPage : System.Web.UI.UserControl
{
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            BindDigitalGoods();
        }
    }

    private void BindDigitalGoods()
    {
        OrderItemDigitalGoodCollection digitalGoodsCollection = GetDigitalGoods();
        if (digitalGoodsCollection.Count > 0)
        {
            DigitalGoodsGrid.DataSource = GetDigitalGoods();
            DigitalGoodsGrid.DataBind();
        }
    }

    private OrderItemDigitalGoodCollection GetDigitalGoods()
    {
        return OrderItemDigitalGoodDataSource.LoadForUser(Token.Instance.UserId);
    }

    protected string GetMaxDownloads(object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        if (oidg.DigitalGood == null) return string.Empty;
        if (oidg.DigitalGood.MaxDownloads == 0) return "(no max)";
        return string.Format("(max {0})", oidg.DigitalGood.MaxDownloads);
    }

    protected string GetDownloadUrl(object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        if (oidg.DownloadStatus == DownloadStatus.Valid)
        {
            return Token.Instance.Store.StoreUrl + "Members/Download.ashx?id=" + oidg.OrderItemDigitalGoodId.ToString();
        }
        return string.Empty;
    }

    protected bool ShowSerialKey(object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        if (oidg.DownloadStatus == DownloadStatus.Valid)
        {
            if (string.IsNullOrEmpty(oidg.SerialKeyData) || oidg.SerialKeyData.Length <= 100)
            {
                return true;
            }
        }
        return false;
    }

    protected bool ShowSerialKeyLink(object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        if (oidg.DownloadStatus == DownloadStatus.Valid)
        {
            if (!string.IsNullOrEmpty(oidg.SerialKeyData) && oidg.SerialKeyData.Length > 100)
            {
                return true;
            }
        }
        return false;
    }

    protected string GetPopUpScript(object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        string url = string.Format("~/Members/MySerialKey.aspx?OrderItemDigitalGoodId={0}", oidg.OrderItemDigitalGoodId);
        url = this.ResolveClientUrl(url);
        string clientScript = string.Format("window.open('{0}'); return false;", url);
        clientScript = "javascript:" + clientScript;
        return clientScript;
    }
}
