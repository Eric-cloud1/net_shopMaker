<%@ WebHandler Language="C#" Class="BuyProduct" %>

using System;
using System.Collections.Generic;
using System.Web;
using MakerShop.Common;
using MakerShop.DigitalDelivery;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Utility;
using MakerShop.Payments;

public class BuyProduct : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        string lastShoppingUrl = NavigationHelper.GetLastShoppingUrl();
        //GET THE PRODUCT ID FROM THE URL
        int productId = AlwaysConvert.ToInt(context.Request.QueryString["p"]);
        Product product = ProductDataSource.Load(productId);
        if (product != null)
        {
            if (product.HasChoices)
            {
                //CANT ADD DIRECTLY TO BASKET, SEND TO MORE INFO
                context.Response.Redirect(product.NavigateUrl);
            }
            BasketItem basketItem = GetBasketItem(productId);
            // DETERMINE IF THE LICENSE AGREEMENT MUST BE REQUESTED
            BasketItemLicenseAgreementCollection basketItemLicenseAgreements = new BasketItemLicenseAgreementCollection(basketItem, LicenseAgreementMode.OnAddToBasket);
            if ((basketItemLicenseAgreements.Count > 0))
            {
                // THESE AGREEMENTS MUST BE ACCEPTED TO ADD TO BASKET
                List<BasketItem> basketItems = new List<BasketItem>();
                basketItems.Add(basketItem);
                string guidKey = Guid.NewGuid().ToString("N");
                context.Cache.Add(guidKey, basketItems, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 10, 0), System.Web.Caching.CacheItemPriority.NotRemovable, null);
                string acceptUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("~/Basket.aspx"));
                string declineUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(lastShoppingUrl));
                context.Response.Redirect("~/BuyWithAgreement.aspx?Items=" + guidKey + "&AcceptUrl=" + acceptUrl + "&DeclineUrl=" + declineUrl);
            }
            Basket basket = Token.Instance.User.Basket;
            basket.Items.Add(basketItem);
            basket.Save();
            basket.Package();
            basket.Combine();
	    //Determine if there are associated Upsell products
	    if (basketItem.Product.GetUpsellProducts(basket).Count > 0)
	    {
	     	//redirect to upsell page
		string returnUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(lastShoppingUrl));
		context.Response.Redirect("ProductAccessories.aspx?ProductId=" + basketItem.ProductId + "&ReturnUrl=" + returnUrl);
	    }

            context.Response.Redirect("~/Basket.aspx");
        }
        context.Response.Redirect(lastShoppingUrl);
    }

    private BasketItem GetBasketItem(int productId)
    {
        BasketItem basketItem = BasketItemDataSource.CreateForProduct(productId, 1, PaymentTypes.Initial);
        return basketItem;
    }
 
    public bool IsReusable {
        get {
            return true;
        }
    }

}