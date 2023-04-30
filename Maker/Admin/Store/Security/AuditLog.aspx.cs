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
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Reporting;
using System.Collections.Generic;

public partial class Admin_Store_Security_AuditLog : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        _OrderUrl = NavigationHelper.GetAdminUrl("Orders/ViewOrder.aspx");
        if (!Page.IsPostBack)
        {
            Logger.Audit(AuditEventType.ViewAuditLog, true, string.Empty);
        }
    }
    
    private string _OrderUrl;

    protected void AuditEventGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            AuditEvent auditEvent = (AuditEvent)e.Row.DataItem;
            if (auditEvent.EventType == AuditEventType.ViewCardData)
            {
                Order order = OrderDataSource.Load(auditEvent.RelatedId);
                if (order != null)
                {
                    PlaceHolder phRe = (PlaceHolder)e.Row.FindControl("phRe");
                    if (phRe != null)
                    {
                        HyperLink link = new HyperLink();
                        link.NavigateUrl = _OrderUrl + "?OrderNumber=" + order.OrderNumber.ToString() + "&OrderId=" + order.OrderId.ToString();
                        link.Text = "Order #" + order.OrderNumber.ToString();
                        phRe.Controls.Add(link);
                    }
                }
            }
        }
    }

}
