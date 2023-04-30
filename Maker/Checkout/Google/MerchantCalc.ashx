<%@ WebHandler Language="C#" Class="MerchantCalc" %>

using System;
using System.Web;
using System.IO;
using System.Security.Principal;
using MakerShop.Payments.Providers.GoogleCheckout.MerchantCalculation;
using MakerShop.Payments.Providers.GoogleCheckout.AC;
using MakerShop.Payments.Providers.GoogleCheckout.Util;

public class MerchantCalc : BasicAuthenticationHandler
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
            
            try
            {
                // Extract the XML from the request.
                Stream requestStream = request.InputStream;
                StreamReader requestStreamReader = new StreamReader(requestStream);
                string requestXml = requestStreamReader.ReadToEnd();
                requestStream.Close();

                //Log.Debug("Request XML: " + requestXml);

                // Process the incoming XML.
                CallbackProcessor P = new CallbackProcessor(new AcCallbackRules());
                byte[] ResponseXML = P.Process(requestXml);

                //Log.Debug("Response XML: " + EncodeHelper.Utf8BytesToString(ResponseXML));

                response.BinaryWrite(ResponseXML);
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
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

}
