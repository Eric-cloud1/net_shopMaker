using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Catalog;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Reporting;
using MakerShop.Stores;
using System.Collections.Generic;
using MakerShop.Personalization;
using MakerShop.Web;
using MakerShop.Web.UI.WebControls.WebParts;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// Utility methods to assist with navigation around the store when URIs may require dynamic construction.
/// </summary>
public static class NavigationHelper
{
    /// <summary>
    /// Gets the current URL encoded for inclusion in the query string.
    /// </summary>
    public static string GetEncodedReturnUrl()
    {
        string returnUrl = NavigationHelper.GetLastShoppingUrl();
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(returnUrl));
    }

    /// <summary>
    /// checks for a return url specified in the query string
    /// </summary>
    /// <param name="defaultUrl">default value if no url found in query string</param>
    /// <returns>The return url specified, or the default url if none found in query string.</returns>
    public static string GetReturnUrl(string defaultUrl)
    {
        HttpContext context = HttpContext.Current;
        if (context == null) return defaultUrl;
        HttpRequest request = context.Request;
        string encodedUrl = request.QueryString["ReturnUrl"];
        string decodedUrl;
        try
        {
            decodedUrl = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedUrl));
        }
        catch
        {
            decodedUrl = encodedUrl;
        }
        if (string.IsNullOrEmpty(decodedUrl)) decodedUrl = defaultUrl;
        string baseUrl;
        if (!request.IsSecureConnection)
        {
            int port = request.Url.Port;
            if (port == 80) baseUrl = "http://" + request.Url.Authority;
            else baseUrl = "http://" + request.Url.Authority + ":" + port.ToString();
        }
        else
        {
            int port = request.Url.Port;
            if (port == 443) baseUrl = "https://" + request.Url.Authority;
            else baseUrl = "https://" + request.Url.Authority + ":" + port.ToString();
        }
        if (decodedUrl.StartsWith(baseUrl)) decodedUrl = decodedUrl.Substring(baseUrl.Length);
        return decodedUrl;
    }

    /// <summary>
    /// Gets the base admin URL
    /// </summary>
    /// <returns>A string containing the base admin url</returns>
    public static string GetAdminUrl()
    {
        return GetAdminUrl(string.Empty);
    }

    /// <summary>
    /// Gets the admin url for the given path
    /// </summary>
    /// <param name="path">The path to the admin file, relative to the base admin folder.</param>
    /// <returns>A string containing the admin url</returns>
    public static string GetAdminUrl(string path)
    {
        return "~/Admin/" + path;
    }

    /// <summary>
    /// Gets the url to the store home page
    /// </summary>
    /// <returns>A string containing the url to the store home page</returns>
    public static string GetStoreUrl(Page page)
    {
        return page.ResolveClientUrl("~/Default.aspx");
    }

    /// <summary>
    /// Gets the url for the given path
    /// </summary>
    /// <param name="path">The path to the file, relative to the store folder.</param>
    /// <returns>A string containing the admin url</returns>
    public static string GetStoreUrl(Page page, string path)
    {
        return page.ResolveClientUrl("~/" + path);
    }

    /// <summary>
    /// Gets the url to the store homepage.
    /// </summary>
    /// <returns>A string containting the url to the store homepage</returns>
    public static string GetHomeUrl()
    {
        return "~/Default.aspx";
    }

    /// <summary>
    /// Gets the url required to begin checkout
    /// </summary>
    /// <returns>A string containing the checkout url</returns>
    public static string GetBasketUrl()
    {
        return "~/Basket.aspx";
    }

    /// <summary>
    /// Gets the url for setting ship methods within the checkout process.
    /// </summary>
    /// <returns>A string containing the ship methods url</returns>
    public static string GetShipMethodUrl()
    {
        return "~/Checkout/ShipMethod.aspx";
    }

    /// <summary>
    /// Gets the url for submitting payment within the checkout process.
    /// </summary>
    /// <returns>A string containing the payment url</returns>
    public static string GetPaymentUrl()
    {
        return "~/Checkout/Payment.aspx";
    }

    /// <summary>
    /// Gets the url required to view an order.
    /// </summary>
    /// <param name="orderId">The ID of the order to be viewed.</param>
    /// <returns>A string containing the view order url</returns>
    public static string GetViewOrderUrl(int orderId)
    {
        int orderNumber = OrderDataSource.LookupOrderNumber(orderId);
        return string.Format("~/Members/MyOrder.aspx?OrderNumber={0}&OrderId={1}", orderNumber, orderId);
    }

    /// <summary>
    /// Gets the url to display a receipt to the customer.
    /// </summary>
    /// <param name="orderId">The ID of the order to display a receipt for.</param>
    /// <returns>A string containing the url to display an order receipt</returns>
    public static string GetReceiptUrl(int orderId)
    {
        int orderNumber = OrderDataSource.LookupOrderNumber(orderId);
        return string.Format("~/Checkout/Receipt.aspx?OrderNumber={0}&OrderId={1}", orderNumber, orderId);
    }

    /// <summary>
    /// Gets the url required to begin checkout
    /// </summary>
    /// <returns>A string containing the checkout url</returns>
    public static string GetCheckoutUrl()
    {       
        return GetCheckoutUrl(HttpContext.Current.User.Identity.IsAuthenticated);        
    }

    /// <summary>
    /// Gets the url required to begin checkout
    /// </summary>
    /// <param name="isAuthenticated">A flag indicating whether the URL should be returned for an authenticated user.</param>
    /// <returns>A string containing the checkout url</returns>
    /// <remarks>Authenticated users may have a different checkout starting point than anonymous users.</remarks>
    public static string GetCheckoutUrl(bool isAuthenticated)
    {

        if (!WebflowManager.IsUsingOnePageCheckout())
        {
            if (isAuthenticated)
            {
                return "~/Checkout/EditBillAddress.aspx";
            }
        }
        
        return "~/Checkout/Default.aspx";
    }

    /// <summary>
    /// Gets the url required to begin checkout
    /// </summary>
    /// <param name="isAuthenticated">A flag indicating whether the URL should be returned for an authenticated user.</param>
    /// <returns>A string containing the checkout url</returns>
    /// <remarks>Authenticated users may have a different checkout starting point than anonymous users.</remarks>
    public static string GetCheckoutUrl(bool isAuthenticated, bool isShipAddress)
    {
        if (!WebflowManager.IsUsingOnePageCheckout())
        {
            if (isAuthenticated)
            {
                // FIND IF THE USER HAS A VALID ADDRESS
                bool hasValidAddress = false;
                foreach (MakerShop.Users.Address address in Token.Instance.User.Addresses)
                {
                    if (address.IsValid)
                    {
                        hasValidAddress = true;
                        break;
                    }
                }

                if (hasValidAddress && isShipAddress) return "~/Checkout/ShipAddress.aspx";
                else
                    if (hasValidAddress && !isShipAddress) return "~/Checkout/ShipMethod.aspx";
                    else
                    {
                        int addressId = Token.Instance.User.PrimaryAddress.AddressId;
                        if (addressId != 0) return "~/Checkout/EditShipAddress.aspx?AddressId=" + addressId.ToString();
                        return "~/Checkout/EditShipAddress.aspx";
                    }
            }
        }
        return "~/Checkout/Default.aspx";
        
    }

    /// <summary>
    /// Gets the last shopping url for the current user.
    /// </summary>
    /// <returns>The last shopping url for the current user.</returns>
    public static string GetLastShoppingUrl()
    {
        HttpContext context = HttpContext.Current;
        if (context == null) return "~/Default.aspx";
        Page page = context.Handler as Page;
        if (page == null) return "~/Default.aspx";
        PageViewCollection pageViews = Token.Instance.User.PageViews;
        pageViews.Sort("ActivityDate", GenericComparer.SortDirection.DESC);
        string homePage = page.ResolveUrl("~/Default.aspx");
        string searchPage = page.ResolveUrl("~/Search.aspx");
        string wishlistPage = page.ResolveUrl("~/ViewWishlist.aspx");
        foreach (PageView pageView in pageViews)
        {
            if ((pageView.UriStem == homePage) || (pageView.UriStem == searchPage) || (pageView.UriStem == wishlistPage)) return pageView.UriStem + "?" + pageView.UriQuery;
            if (pageView.CatalogNodeId != 0)
            {
                ICatalogable node = pageView.CatalogNode;
                if ((node != null) && ((pageView.CatalogNodeType == CatalogNodeType.Category) || (pageView.CatalogNodeType == CatalogNodeType.Product)))
                {
                    return node.NavigateUrl;
                }
            }
        }
        return homePage;
    }

    /// <summary>
    /// Given a product, returns the appropriate URL for the rating image.
    /// </summary>
    /// <param name="dataItem">A product instance.</param>
    /// <returns>A string with the URL to the appropriate rating image.</returns>
    public static string GetRatingImage(object product)
    {
        Product tempProduct = (Product)product;
        return GetRatingImage(tempProduct.Rating);
    }

    /// <summary>
    /// Given a rating value, returns the appropriate URL for the rating image.
    /// </summary>
    /// <param name="rating">The rating value (between 0 and 10)</param>
    /// <returns>A string with the URL to the appropriate rating image.</returns>
    public static string GetRatingImage(LSDecimal rating)
    {
        //RATING SHOULD BE BETWEEN 0 AND 10
        int tempRating = ((int)Math.Round((Decimal)rating, 0)) * 10;
        Page page = (Page)HttpContext.Current.Handler;
        string ratingImage = page.ResolveUrl("~/images/ratings/rate_" + tempRating.ToString() + ".gif");
        return ratingImage;
    }

    public static string GetPublishIconUrl(object catalogNode)
    {
        return GetPublishIconUrl(((CatalogNode)catalogNode).Visibility);
    }

    public static string GetPublishIconUrl(CatalogVisibility visibility)
    {
        HttpContext context = HttpContext.Current;
        Page page = context.Handler as Page;
        if (page != null)
        {
            string theme = page.Theme;
            if (string.IsNullOrEmpty(theme)) theme = page.StyleSheetTheme;
            string baseImagesFolder = (string.IsNullOrEmpty(theme) ? "~/Images/Icons/" : "~/App_Themes/" + theme + "/Images/Icons/");
            switch (visibility)
            {
                case CatalogVisibility.Public:
                    return baseImagesFolder + "CmsPublic.png";
                case CatalogVisibility.Hidden:
                    return baseImagesFolder + "CmsHidden.png";
                default:
                    return baseImagesFolder + "CmsPrivate.png";
            }
        }
        return string.Empty;
    }

    public static List<PagerLinkData> GetPaginationLinks(int currentPagerIndex, int totalPages, string baseUrl)
    {
        List<PagerLinkData> pagerLinkData = new List<PagerLinkData>();

        baseUrl += "p=";
        string navigateUrl;
        // CALCULATE THE START AND END INDEX FOR PAGER
        int startPagerIndex = 0;
        int lastPagerIndex = totalPages - 1;

        int pagerRange = 5; // NUMBER OF LINKS BEFORE AND AFTER THE CURRENT PAGE LINK

        if (currentPagerIndex > pagerRange) startPagerIndex = currentPagerIndex - pagerRange;

        if (lastPagerIndex > (currentPagerIndex + pagerRange)) lastPagerIndex = currentPagerIndex + pagerRange;

        if (startPagerIndex > 0)
        {
            navigateUrl = baseUrl + (currentPagerIndex - 1).ToString();
            pagerLinkData.Add(new PagerLinkData("<", navigateUrl, (currentPagerIndex - 1), true));
        }

        int pageIndexCounter = startPagerIndex;
        while (pageIndexCounter <= lastPagerIndex)
        {
            string linkText = ((int)(pageIndexCounter + 1)).ToString();
            if (pageIndexCounter != currentPagerIndex)
            {
                navigateUrl = baseUrl + pageIndexCounter.ToString();
                pagerLinkData.Add(new PagerLinkData(linkText, navigateUrl, pageIndexCounter, (pageIndexCounter != currentPagerIndex)));
            }
            else
            {
                navigateUrl = "#";
                pagerLinkData.Add(new PagerLinkData(linkText, navigateUrl, pageIndexCounter, (pageIndexCounter != currentPagerIndex), "current"));
            }
            pageIndexCounter++;
        }
        if (lastPagerIndex < (totalPages - 1))
        {
            navigateUrl = baseUrl + (currentPagerIndex + 1).ToString();
            pagerLinkData.Add(new PagerLinkData(">", navigateUrl, currentPagerIndex + 1, true));
        }
        return pagerLinkData;
    }

    public static void Trigger404(HttpResponse response, string statusDescription)
    {
        response.Redirect("~/Errors/PageNotFound.aspx");
    }

    public static void Trigger403(HttpResponse response, string statusDescription)
    {
        response.Clear();
        response.Status = "403 Forbidden";
        response.StatusDescription = statusDescription;
        response.End();
    }
}


public class PagerLinkData
{
    private string _Text;
    private int _PageIndex;
    private string _NavigateUrl;
    public int PageIndex { get { return _PageIndex; } }
    private bool _Enabled;
    public string Text { get { return _Text; } }
    public string NavigateUrl { get { return _NavigateUrl; } }
    public bool Enabled { get { return _Enabled; } }
    private string _tagClass;
    public string TagClass { get { return _tagClass; } set { _tagClass = value; } }
    public PagerLinkData(string text, string navigateUrl, int pageIndex, bool enabled)
    {
        _Text = text;
        _NavigateUrl = navigateUrl;
        _PageIndex = pageIndex;
        _Enabled = enabled;
    }

    public PagerLinkData(string text, string navigateUrl, int pageIndex, bool enabled, string tagClass)
    {
        _Text = text;
        _NavigateUrl = navigateUrl;
        _PageIndex = pageIndex;
        _Enabled = enabled;
        _tagClass = tagClass;
    }
}