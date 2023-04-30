<%@ WebHandler Language="C#" Class="viewDetails" %>

using System;
using System.Web;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Common;
using System.Web.UI;

public class viewDetails : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {

        context.Response.Cache.SetNoStore();
        string paymentId = string.Empty, transactionId = string.Empty;

        if (!string.IsNullOrEmpty(context.Request.QueryString["paymentId"]))
            paymentId = context.Request.QueryString["paymentId"];

        if (!string.IsNullOrEmpty(context.Request.QueryString["transactionId"]))
            transactionId = context.Request.QueryString["transactionId"];
        
        
        context.Response.ContentType = "text/plain";

        context.Response.Clear();
        
        if(!string.IsNullOrEmpty(paymentId))
            context.Response.Write("<div class=\"header\"><h2>&nbsp;Payment Id: " + paymentId + "&nbsp;</h2></div>");

        if (!string.IsNullOrEmpty(transactionId))
            context.Response.Write("<div class=\"header\"><h2>&nbsp;Transaction Id: " + transactionId + "&nbsp;</h2></div>");
        
        
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}