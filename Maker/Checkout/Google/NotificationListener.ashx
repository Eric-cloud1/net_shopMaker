<%@ WebHandler Language="C#" Class="NotificationListner" %>

using System;
using System.Web;
using System.IO;
using System.Xml;
using System.Security.Principal;
using MakerShop.Utility;
using MakerShop.Payments.Providers.GoogleCheckout.AC;
using MakerShop.Payments.Providers.GoogleCheckout.Util;
using MakerShop.Payments.Providers.GoogleCheckout.AutoGen;

public class NotificationListner : BasicAuthenticationHandler
{    
    public override void ProcessRequest (HttpContext context) {

        HttpRequest request = context.Request;
        HttpResponse response = context.Response;

        string AuthHeader = request.Headers["Authorization"];
        string userName = GetUserName(AuthHeader);
        string password = GetPassword(AuthHeader);

        if (UserHasAccess(userName, password))
        {
            context.User = new GenericPrincipal(new GenericIdentity(userName, "Google.Checkout.Basic"), new string[1] { "User" });

            // Extract the XML from the request.
            Stream requestStream = request.InputStream;
            StreamReader requestStreamReader = new StreamReader(requestStream);
            string RequestXml = requestStreamReader.ReadToEnd();
            requestStream.Close();
            // Act on the XML.
            string topElement = EncodeHelper.GetTopElement(RequestXml);
            context.Trace.Warn("Process Google Notification: " + topElement);
            switch (topElement)
            {
                case "new-order-notification":
                    NewOrderNotification N1 = (NewOrderNotification)EncodeHelper.Deserialize(RequestXml, typeof(NewOrderNotification));
                    NewOrderHandler nhandler = new NewOrderHandler(N1);
                    nhandler.Process();
                    break;
                case "risk-information-notification":
                    RiskInformationNotification N2 = (RiskInformationNotification)EncodeHelper.Deserialize(RequestXml, typeof(RiskInformationNotification));
                    RiskInformationHandler rhandler = new RiskInformationHandler(N2);
                    rhandler.Process();
                    break;
                case "order-state-change-notification":
                    OrderStateChangeNotification N3 = (OrderStateChangeNotification)EncodeHelper.Deserialize(RequestXml, typeof(OrderStateChangeNotification));
                    OrderStateChangeHandler osHandler = new OrderStateChangeHandler(N3);
                    osHandler.Process();
                    break;
                case "charge-amount-notification":
                    ChargeAmountNotification N4 = (ChargeAmountNotification)EncodeHelper.Deserialize(RequestXml, typeof(ChargeAmountNotification));
                    ChargeAmountHandler caHandler = new ChargeAmountHandler(N4);
                    caHandler.Process();
                    break;
                case "refund-amount-notification":
                    RefundAmountNotification N5 = (RefundAmountNotification)EncodeHelper.Deserialize(RequestXml, typeof(RefundAmountNotification));
                    RefundAmountHandler rfHandler = new RefundAmountHandler(N5);
                    rfHandler.Process();
                    break;
                case "chargeback-amount-notification":
                    ChargebackAmountNotification N6 = (ChargebackAmountNotification)EncodeHelper.Deserialize(RequestXml, typeof(ChargebackAmountNotification));
                    ChargebackAmountHandler cbHandler = new ChargebackAmountHandler(N6);
                    cbHandler.Process();
                    break;
                default:
                    break;
            }

            SendAcknowledgment(response);
        }
        else
        {
            response.ClearContent();             
            response.SuppressContent = true;
            response.StatusCode = 401;
            response.StatusDescription = "Access Denied";
            response.AppendHeader("WWW-Authenticate", "Basic Realm=\"CheckoutCallbackRealm\"");
            response.Flush();            
            context.ApplicationInstance.CompleteRequest();
        }
    }
 
    public override bool IsReusable {
        get {
            return true;
        }
    }

    private void SendAcknowledgment(HttpResponse response)
    {
        response.Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        response.Output.WriteLine("<notification-acknowledgment xmlns=\"http://checkout.google.com/schema/2\"/>");
    }
}
