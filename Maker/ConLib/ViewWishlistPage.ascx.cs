using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using MakerShop.Common;
using MakerShop.Products;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Catalog;
using MakerShop.Payments;

public partial class ConLib_ViewWishlistPage : System.Web.UI.UserControl
{
    int _WishlistId = 0;
    Wishlist _Wishlist;

    private string _Caption = "{0}'s Wishlist";
    public string Caption
    {
        get { return _Caption; }
        set { _Caption = value; }
    }

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _WishlistId = AlwaysConvert.ToInt(Request.QueryString["WishlistId"]);
        _Wishlist = WishlistDataSource.Load(_WishlistId);
        if (_Wishlist == null) 
        {
            Response.Redirect(NavigationHelper.GetHomeUrl());
            return;
        }

        _Wishlist.Recalculate();
        WishlistItemCollection items =  _Wishlist.Items;
        Product product;
        WishlistItem item;
        for(int i=items.Count-1; i>=0; i--)
        {
            item = items[i];
            product = item.Product;
            if(product == null || product.Visibility == CatalogVisibility.Private || isVariantInvalid(item)) 
            {
                items.RemoveAt(i);
            }
        }

        WishlistGrid.DataSource = items;
        WishlistGrid.DataBind();

        if (!string.IsNullOrEmpty(_Wishlist.ViewPassword))
        {
            string currentPassword = (string)Session["Wishlist" + _WishlistId.ToString() + "_Password"];
            if ((currentPassword == null) || (currentPassword != _Wishlist.ViewPassword))
            {
                WishlistMultiView.SetActiveView(PasswordView);
                PageHelper.SetDefaultButton(Password, CheckPasswordButton.ClientID);
            }
        }
        if (WishlistMultiView.ActiveViewIndex == 0)
        {
            WishlistCaption.Text = string.Format(_Caption, GetUserName(_Wishlist.User));
        }
        else
        {
            WishlistCaption.Text = "Enter Wishlist Password";
        }
    }

    private bool isVariantInvalid(WishlistItem item)
    {
        if(item.Product.ProductOptions.Count > 0) 
        {
            ProductVariant v = ProductVariantDataSource.LoadForOptionList(item.ProductId, item.OptionList);
            if (v == null || !v.Available) return true;
        }

        return false;
    }

    protected void CheckPasswordButton_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Password.Text))
        {
            string currentPassword = Password.Text;
            if (_Wishlist.ViewPassword == currentPassword)
            {
                Session["Wishlist" + _WishlistId.ToString() + "_Password"] = currentPassword;
                WishlistMultiView.SetActiveView(WishlistView);
                WishlistCaption.Text = string.Format(_Caption, GetUserName(_Wishlist.User));
            }
            else
            {
                InvalidPasswordLabel.Visible = true;
                PasswordLabel.Visible = false;
            }
        }
    }

    protected void WishlistGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Basket")
        {
            int wishlistItemId = AlwaysConvert.ToInt(e.CommandArgument);
            int index = _Wishlist.Items.IndexOf(wishlistItemId);
            if (index > -1)
            {
                _Wishlist.Items.CopyToBasket(index, Token.Instance.User.Basket);
                if (!PageHelper.HasBasketControl(this.Page)) Response.Redirect("~/Basket.aspx");
            }
        }
    }

    protected string GetPriorityString(object priority)
    {
        switch ((byte)priority)
        {
            case 0:
                return "lowest";
            case 1:
                return "low";
            case 2:
                return "medium";
            case 3:
                return "high";
            case 4:
                return "highest";
        }
        return string.Empty;
    }

    private string GetUserName(User u)
    {
        if (u == null) return string.Empty;
        if (u.IsAnonymous) return "Anonymous";
        string fullName = u.PrimaryAddress.FullName;
        if (!string.IsNullOrEmpty(fullName)) return fullName;
        return u.UserName;
    }

    protected bool HasKitProducts(object dataItem)
    {
        return !string.IsNullOrEmpty(((WishlistItem)dataItem).KitList);
    }

    protected List<KitProduct> GetKitProducts(object dataItem)
    {
        return ((WishlistItem)dataItem).GetKitProducts(false);
    }

    protected LSDecimal GetPrice(object dataItem)
    {
        WishlistItem item = (WishlistItem)dataItem;
        //DETERMINE THE BASE PRICE OF THE ITEM
        LSDecimal price;
        if (item.Product.UseVariablePrice)
        {
            price = item.Price;
            if (price < item.Product.MinimumPrice) price = item.Product.MinimumPrice;
            if (price > item.Product.MaximumPrice) price = item.Product.MaximumPrice;
            item.Price = price;
        }
        else
        {
            // ADD PRICE OF KIT PRODUCTS AS WELL
            ProductCalculator c = ProductCalculator.LoadForProduct(item.ProductId, 1, item.OptionList, item.KitList, PaymentTypes.Initial);
            price = c.Price;
        }
        return price;
    }

}
