<%@ WebHandler Language="C#" Class="IdentifyBrowser" %>

using System;
using System.Web;

public class IdentifyBrowser : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        HttpBrowserCapabilities browser = context.Request.Browser;
        if (browser.Browser != "Unknown")
            context.Response.Write(browser.Browser + " " + browser.MajorVersion + " " + browser.Platform);
        else context.Response.Write(browser.Browser);
    
    }
 
    public bool IsReusable {
        get {
            return true;
        }
    }

}