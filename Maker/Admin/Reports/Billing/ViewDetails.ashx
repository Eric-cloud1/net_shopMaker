<%@ WebHandler Language="C#" Class="ViewDetails" %>

using System;
using System.Web;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Common;
using System.Web.UI;

public class ViewDetails : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.Cache.SetNoStore();
        DateTime? paymentDate = null, orderDate = null, transactionDate = null, endDate = null;
        string gateway = string.Empty;
        
        DateTime dt;
        if (DateTime.TryParse(context.Request.QueryString["TransactionDate"], out dt))
            transactionDate = dt;
        if (DateTime.TryParse(context.Request.QueryString["PaymentDate"], out dt))
            paymentDate = dt;
        if (DateTime.TryParse(context.Request.QueryString["OrderDate"], out dt))
            orderDate = dt;
        if (DateTime.TryParse(context.Request.QueryString["EndDate"], out dt))
            endDate = dt;

        


        string parameters = string.Empty;

        if (transactionDate.HasValue)
            parameters += "&TransactionDate=" + HttpUtility.UrlEncode(transactionDate.Value.ToString());
        if (paymentDate.HasValue)
            parameters += "&PaymentDate=" + HttpUtility.UrlEncode(paymentDate.Value.ToString());
        if (orderDate.HasValue)
            parameters += "&OrderDate=" + HttpUtility.UrlEncode(orderDate.Value.ToString());
        if (endDate.HasValue)
            parameters += "&EndDate=" + HttpUtility.UrlEncode(endDate.Value.ToString());


        if (!string.IsNullOrEmpty(context.Request.QueryString["Gateway"]))
            parameters += "&paymentGatewayPrefix=" + context.Request.QueryString["Gateway"].Trim();


        if (!string.IsNullOrEmpty(context.Request.QueryString["country"]))
            parameters += "&country=" + context.Request.QueryString["country"].Trim();
        

        if (!string.IsNullOrEmpty(context.Request.QueryString["reportType"]))
            parameters += "&reportType=" + context.Request.QueryString["reportType"].Trim();

      if (!string.IsNullOrEmpty(context.Request.QueryString["paymentTypes"]))
         parameters += "&paymentTypes=" + context.Request.QueryString["paymentTypes"].Trim();

      if (!string.IsNullOrEmpty(context.Request.QueryString["affiliates"]))
          parameters += "&affiliates=" + context.Request.QueryString["affiliates"].Trim();

      if (!string.IsNullOrEmpty(context.Request.QueryString["subaffiliates"]))
          parameters += "&subaffiliates=" + context.Request.QueryString["subaffiliates"].Trim();


      if (!string.IsNullOrEmpty(context.Request.QueryString["paymentGatewayPrefix"]))
          parameters += "&paymentGatewayPrefix=" + context.Request.QueryString["paymentGatewayPrefix"].Trim();
        
        
        
     
            

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

        context.Response.Write("<ul><li><a target='Transaction' href='" + transactionUrl + "?a=0");
        context.Response.Write(parameters);

        context.Response.Write("'>View Details</a></li>");


        int i;
        if (int.TryParse(context.Request.QueryString["reportType"].ToString(), out i) && (Token.Instance.User.IsSecurityAdmin))
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


