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
        string orderNumber = string.Empty;

        if (!string.IsNullOrEmpty(context.Request.QueryString["orderNumber"]))
            orderNumber = context.Request.QueryString["orderNumber"];


        
        context.Response.ContentType = "text/plain";

        context.Response.Clear();

        if (!string.IsNullOrEmpty(orderNumber))
            context.Response.Write("<div class=\"header\"><h2>&nbsp;Order Number: " + orderNumber + "&nbsp;</h2></div>");

      
        
        
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}