<%@ WebHandler Language="C#" Class="ViewIdsDetails" %>

using System;
using System.Web;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Common;
using System.Web.UI;

public class ViewIdsDetails : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.Cache.SetNoStore();
        DateTime? paymentDate = null, orderDate = null, transactionDate = null, endDate = null;
        string gateway = string.Empty;
        string report = string.Empty;

        string parameters = string.Empty;

        if (!string.IsNullOrEmpty(context.Request.QueryString["ids"]))
            parameters = Helper.MISC.base64Decode(context.Request.QueryString["ids"].Replace("-","="));

        System.Collections.Specialized.NameValueCollection values = System.Web.HttpUtility.ParseQueryString(parameters.ToLower());
        
  
        DateTime dt;
        if (DateTime.TryParse(values["transactiondate"], out dt))
            transactionDate = dt;

        if (DateTime.TryParse(values["paymentdate"], out dt))
            paymentDate = dt;

        if (DateTime.TryParse(values["orderdate"], out dt))
            orderDate = dt;

        if (DateTime.TryParse(values["enddate"], out dt))
            endDate = dt;
        
        

        context.Response.Clear();

        //Page.ResolveClientUrl

        context.Response.Write("<div class=\"section\" style=\"width:200px\">\r\n");
        //  context.Response.Write("ReportId: " + reportType);
        if (orderDate.HasValue)
            context.Response.Write("<div class=\"header\"><h2>Order Date: " + orderDate.Value.ToString("MM/dd/yy") + "</h2></div>");
        if (paymentDate.HasValue)
            context.Response.Write("<div class=\"header\"><h2>Payment Date: " + paymentDate.Value.ToString("MM/dd/yy") + "</h2></div>");
        if (transactionDate.HasValue)
            context.Response.Write("<div class=\"header\"><h2>Transaction Date: " + transactionDate.Value.ToString("MM/dd/yy") + "</h2></div>");
        context.Response.Write("<div class=\"header\"><h3>Action(s): </h3></div>");
        context.Response.Write("<div class=\"itemList\" style='border: solid 1px gray; border-color:gray; background-color:White; padding:6px;text-align:left; cursor: help;'>");
       
        Control c = new Control();

        string transactionUrl = c.ResolveUrl("~/Admin/Reports/Billing/Transaction.aspx");

        context.Response.Write("<ul><li><a target='Transaction' href='" + transactionUrl + "?a=0&");
        context.Response.Write(parameters);

        context.Response.Write("'>View Details</a></li>");

        if(!string.IsNullOrEmpty(values["reporttype"].ToString()))
            report = values["reporttype"].ToString();  
        
        int i;
        if (int.TryParse(report, out i) && (Token.Instance.User.IsSecurityAdmin))
        {
            if ((i >= 22) && (i <= 27))
            {
                context.Response.Write("<li><a target='Reschedule' href='" + c.ResolveClientUrl("~/Admin/Payment/Reschedule.aspx") +"?type=Authorize&reschedule=1");
                context.Response.Write(parameters);
                context.Response.Write("'>Reschedule Authorize</a></li>");
            }
            else if ((i >= 28) && (i <= 32)||(i ==34))
            {
                context.Response.Write("<li><a target='Capture' href='" + c.ResolveClientUrl("~/Admin/Payment/Reschedule.aspx") + "?type=Capture&reschedule=1");
                context.Response.Write(parameters);
                context.Response.Write("'>Reschedule Capture</a></li>");

            }
        }
        context.Response.Write("</ul></div>");
        context.Response.Write("</div>");



        context.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}


