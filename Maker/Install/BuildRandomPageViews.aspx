<%@ Page Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">
    private Random rand = new Random();

    private void SetBrowser(PageView pageView)
    {
        int browserId = rand.Next(0, 10);
        switch (browserId)
        {
            case 0:
                pageView.UserAgent = "Mozilla/5.0 (Windows; U; Win98; en-US; rv:1.4) Gecko Netscape/7.1 (ax)";
                pageView.BrowserName = "Netscape";
                pageView.BrowserPlatform = "WinXP";
                pageView.BrowserVersion = "7.1";
                pageView.Browser = "Netscape 7 WinXP";
                break;
            case 1:
                pageView.UserAgent = "Opera/8.51 (Windows NT 5.1; U; en)";
                pageView.BrowserName = "Opera";
                pageView.BrowserPlatform = "WinXP";
                pageView.BrowserVersion = "8.51";
                pageView.Browser = "Opera 8 WinXP";
                break;
            case 2:
            case 3:
            case 4:
                pageView.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.6) Gecko/20070725 Firefox/2.0.0.6";
                pageView.BrowserName = "Firefox";
                pageView.BrowserPlatform = "WinXP";
                pageView.BrowserVersion = "2.0.0.6";
                pageView.Browser = "Firefox 2 WinXP";
                break;
            case 5:
            case 6:
                pageView.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                pageView.BrowserName = "IE";
                pageView.BrowserPlatform = "WinXP";
                pageView.BrowserVersion = "6.0";
                pageView.Browser = "IE 6 WinXP";
                break;
            case 7:
            case 8:
            case 9:
                pageView.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                pageView.BrowserName = "IE";
                pageView.BrowserPlatform = "WinXP";
                pageView.BrowserVersion = "7.0";
                pageView.Browser = "IE 7 WinXP";
                break;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Write("Generating page views .");
        int productViews = AlwaysConvert.ToInt(Request.QueryString["products"]);
        if (productViews < 1) productViews = 1;
        for (int i = 0; i < productViews; i++)
        {
            ProductCollection products = ProductDataSource.LoadForCriteria("VisibilityId = 0", 100, 0, string.Empty);
            int prodIndex = rand.Next(products.Count - 1);
            Product product = products[prodIndex];
            int daysAgo = rand.Next(0, 14);
            int minutesAgo = rand.Next(0, (24 * 60));
            DateTime viewDate = DateTime.UtcNow.AddDays((-1 * daysAgo)).AddMinutes((-1 * minutesAgo));
            PageView pageView = new PageView();
            pageView.ActivityDate = viewDate;
            pageView.TimeTaken = rand.Next(0, 201);
            pageView.UserId = 0;
            pageView.UriStem = Request.Url.AbsolutePath.ToLowerInvariant().Replace("/install/buildrandompageviews.aspx", product.NavigateUrl.TrimStart("~".ToCharArray()));
            pageView.UriQuery = string.Empty;
            pageView.RequestMethod = Request.RequestType;
            pageView.RemoteIP = "127.0.0.1";
            SetBrowser(pageView);
            pageView.CatalogNodeId = product.ProductId;
            pageView.CatalogNodeTypeId = (byte)CatalogNodeType.Product;
            pageView.Save();
            Response.Write(" .");
            Response.Flush();
        }

        int categoryViews = AlwaysConvert.ToInt(Request.QueryString["categories"]);
        if (categoryViews < 1) categoryViews = 1;
        for (int i = 0; i < categoryViews; i++)
        {
            CategoryCollection categories = CategoryDataSource.LoadForCriteria("VisibilityId = 0", 100, 0, string.Empty);
            int catIndex = rand.Next(categories.Count - 1);
            Category category = categories[catIndex];
            int daysAgo = rand.Next(0, 14);
            int minutesAgo = rand.Next(0, (24 * 60));
            DateTime viewDate = DateTime.UtcNow.AddDays((-1 * daysAgo)).AddMinutes((-1 * minutesAgo));
            PageView pageView = new PageView();
            pageView.ActivityDate = viewDate;
            pageView.TimeTaken = rand.Next(0, 201);
            pageView.UserId = 0;
            pageView.UriStem = Request.Url.AbsolutePath.ToLowerInvariant().Replace("/install/buildrandompageviews.aspx", category.NavigateUrl.TrimStart("~".ToCharArray()));
            pageView.UriQuery = string.Empty;
            pageView.RequestMethod = Request.RequestType;
            pageView.RemoteIP = "127.0.0.1";
            SetBrowser(pageView);
            pageView.CatalogNodeId = category.CategoryId;
            pageView.CatalogNodeTypeId = (byte)CatalogNodeType.Category;
            pageView.Save();
            Response.Write(" .");
            Response.Flush();
        }

        Response.Write(" DONE!");
        Response.End();
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Page Views Generated
    </div>
    </form>
</body>
</html>
