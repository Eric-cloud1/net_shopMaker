<%@ WebHandler Language="C#" Class="Download" %>

using System;
using System.Web;
using MakerShop.Common;
using MakerShop.DigitalDelivery;
using MakerShop.Orders;
using MakerShop.Utility;

public class Download : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Server.ScriptTimeout = 14400;
        HttpResponse Response = context.Response;
        //LOAD REQUESTED DIGITAL GOOD
        int orderItemDigitalGoodId = -1;
        OrderItemDigitalGood oidg = null;
        if (context.Request.QueryString["type"] == null)
        {
            orderItemDigitalGoodId = AlwaysConvert.ToInt(context.Request.QueryString["id"]);
            oidg = OrderItemDigitalGoodDataSource.Load(orderItemDigitalGoodId);
        }
        else if (context.Request.QueryString["type"] == "ordernumber")
        {
            int orderid = OrderDataSource.LookupOrderId(AlwaysConvert.ToInt(context.Request.QueryString["id"]));
            foreach (OrderItem oi in OrderItemDataSource.LoadForOrder(orderid))
            {
                foreach (OrderItemDigitalGood OIDG in oi.DigitalGoods)
                {// get the first one
                    oidg = OIDG;
                    break;
                }
            }
        }

        //VERIFY DIGITAL GOOD IS VALID
        if (oidg != null)
        {
            //VERIFY REQUESTING USER PLACED THE ORDER
            OrderItem orderItem = oidg.OrderItem;
            if (orderItem != null)
            {
                Order order = orderItem.Order;
                if (order != null)
                {
                    //  if (Token.Instance.UserId == order.UserId)
                    if (true)
                    {
                        //VERIFY THE DOWNLOAD IS VALID
                        if (oidg.DownloadStatus == DownloadStatus.Valid)
                        {
                            DigitalGood digitalGood = oidg.DigitalGood;
                            if (digitalGood != null)
                            {
                                //RECORD THE DOWNLOAD
                                Uri referrer = context.Request.UrlReferrer;
                                string referrerUrl = (referrer != null) ? referrer.ToString() : string.Empty;
                                oidg.RecordDownload(context.Request.UserHostAddress, context.Request.UserAgent, referrerUrl);
                                DownloadHelper.SendFileDataToClient(context, digitalGood);
                            }
                            else
                            {
                                Response.Write("The requested file could not be located.");
                            }
                        }
                        else
                        {
                            Response.Write("This download is expired or invalid.");
                        }
                    }
                    else
                    {
                        Response.Write("You are not authorized to download the requested file.");
                    }
                }
                else
                {
                    Response.Write("The order could not be loaded.");
                }
            }
            else
            {
                Response.Write("The order item could not be loaded.");
            }
        }
        else
        {
            Response.Write("The requested item does not exist.");
        }
    }

    public bool IsReusable
    {
        get { return true; }
    }
}