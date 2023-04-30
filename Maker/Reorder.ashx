<%@ WebHandler Language="C#" Class="Reorder" %>

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using MakerShop.Common;
using MakerShop.DigitalDelivery;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Payments;

public class Reorder : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        HttpResponse Response = context.Response;
        //GET THE ORDER ID FROM THE URL
        int orderId = AlwaysConvert.ToInt(context.Request.QueryString["o"]);
        Order order = OrderDataSource.Load(orderId);
        if (order != null)
        {
            //MAKE SURE ORDER IS FOR CURRENT USER
            User user = Token.Instance.User;
            if (order.UserId == user.UserId)
            {
                //CLEAR THE EXISTING BASKET
                List<string> basketMessages = new List<string>();
                Basket basket = user.Basket;
                basket.Clear();
                foreach (OrderItem item in order.Items)
                {
                    if ((item.OrderItemType == OrderItemType.Product) && (!item.IsChildItem))
                    {
                        Product product = item.Product;
                        if ((product != null) && (product.Visibility != MakerShop.Catalog.CatalogVisibility.Private))
                        {
                            BasketItem basketItem;
                            try
                            {
                                basketItem = BasketItemDataSource.CreateForProduct(item.ProductId, item.Quantity, item.OptionList, item.KitList, PaymentTypes.Recurring);
                            }
                            catch
                            {
                                string itemName = item.Name;
                                if (!string.IsNullOrEmpty(item.VariantName)) itemName += " (" + item.VariantName +")";
                                basketMessages.Add("The item " + itemName + " is no longer available.");
                                basketItem = null;
                            }
                            if (basketItem != null)
                            {
                                //SEE IF A PRODUCT TEMPLATE IS ASSOCIATED
                                foreach (ProductProductTemplate ppt in product.ProductProductTemplates)
                                {
                                    ProductTemplate template = ppt.ProductTemplate;
                                    if (template != null)
                                    {
                                        foreach (InputField inputField in template.InputFields)
                                        {
                                            if (!inputField.IsMerchantField)
                                            {
                                                //COPY OVER ANY CUSTOMER INPUTS
                                                BasketItemInput itemInput = new BasketItemInput();
                                                itemInput.InputFieldId = inputField.InputFieldId;
                                                itemInput.InputValue = GetItemInputValue(item, inputField.Name);
                                                basketItem.Inputs.Add(itemInput);
                                            }
                                        }
                                    }
                                }
                                if ((basketItem.OrderItemType == OrderItemType.Product) && (basketItem.Product.UseVariablePrice)) basketItem.Price = item.Price;
                                basket.Items.Add(basketItem);
                                //WE HAVE TO SAVE THE BASKET IN CASE IT IS NOT YET CREATED
                                basket.Save();
                            }
                        }
                    }
                }
                if (context.Session != null) context.Session["BasketMessage"] = basketMessages;
                Response.Redirect(NavigationHelper.GetBasketUrl());
            }
        }
        Response.Redirect(NavigationHelper.GetHomeUrl());
    }

    private static string GetItemInputValue(OrderItem item, string key)
    {
        foreach (OrderItemInput input in item.Inputs)
        {
            if (input.Name == key) return input.InputValue;
        }
        return string.Empty;
    }

    public bool IsReusable { get { return true; } }
}