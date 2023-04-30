<%@ Application Classname="AcApp" Language="C#" %> 
<script runat="server">
    protected void Application_BeginRequest(object sender, EventArgs e)
    {
        //TERMINATE SQL INJECTION ATTEMPTS
        int maxQueryLength = 500;
        string rawUrl = Request.RawUrl;
        int qIndex = rawUrl.IndexOf("?");
        if (qIndex > -1)
        {
            string query = Request.RawUrl.Substring(qIndex).ToUpperInvariant();
            if (query.Length > maxQueryLength || query.Contains("DECLARE%20"))
            {
                //POTENTIAL ATTACK
                Response.Clear();
                Response.Write("INVALID REQUEST");
                Response.Flush();
                Response.End();
            }
        }
        //CHECK FOR "NEW COOKIE PLEASE" INDICATOR
        string ncp = Request.QueryString["NCP"];
        if (ncp != null)
        {
            HttpCookie authCookie = Response.Cookies["AC7.ASPXAUTH"];
            if (authCookie != null) authCookie.Expires = DateTime.Now.AddYears(-1);
            HttpCookie anonCookie = Response.Cookies["AC7.ASPXANONYMOUS"];
            if (anonCookie != null) anonCookie.Expires = DateTime.Now.AddYears(-1);
            HttpCookie sessionCookie = Response.Cookies["AC7.SESSIONID"];
            if (sessionCookie != null) sessionCookie.Expires = DateTime.Now.AddYears(-1);
            Response.Redirect(Request.Url.AbsolutePath);
        }
    }

    protected void Session_OnStart()
    {
        //SAVE THE REFERRER FOR USE BY THE ORDER MODULE
        if (Request.UrlReferrer != null) Session["SessionReferrerUrl"] = StringHelper.Truncate(Request.UrlReferrer.ToString(), 255);
    }

	protected void Application_Error(Object sender, EventArgs e)
	{
        // ENABLE ERROR LOGGING FOR SCRIPTS OUTSIDE OF THE INSTALL DIRECTORY
        if (!HttpContextHelper.IsInstallRequest())
        {
            // RECORD THE DETAILS TO THE AC ERROR LOG
            HttpContext ctx = HttpContext.Current;
            Exception exception = ctx.Server.GetLastError();

            // IGNORE HttpExceptions
            if (exception is HttpException)
            {
                if ((exception.InnerException == null) || (exception.InnerException is HttpException))
                    return;
            }
            
            // IGNORE INVALID VIEW STATE ERRORS
            if (IsViewStateException(exception)) return;
            
            // IGNORE ERRORS WITHIN AXD RESOURCES
            if (ctx.Request.Path.ToLowerInvariant().EndsWith(".axd")) return;

            string errorInfo = "An error has occured at " + ctx.Request.Url.ToString();
            Logger.Error(errorInfo, exception);
            
          
            String Subject = "Site-Error: " + HttpContext.Current.Request.Path;
            String To = "it@telecomworldus.com";
            VPlex.Common.ErrorHandling.HtmlError.sendHtmlError(exception, To, To, Subject);
        }
        
	}

    private bool IsViewStateException(Exception exception)
    {
        if (exception == null) return false;
        if (exception is ViewStateException) return true;
        return IsViewStateException(exception.InnerException);
    }

</script>
