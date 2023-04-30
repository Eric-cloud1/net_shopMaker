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
using MakerShop.Taxes;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Payments;

public partial class ConLib_MyWishlistPage : System.Web.UI.UserControl
{
    protected void ClearWishlistButton_Click(object sender, EventArgs e)
    {
        Token.Instance.User.PrimaryWishlist.Items.DeleteAll();
        WishlistGrid.DataBind();
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        // build hashtable of new quantities
        Wishlist w = Token.Instance.User.PrimaryWishlist;
        int rowIndex = 0;
        foreach (GridViewRow saverow in WishlistGrid.Rows)
        {
            int wishlistItemId = (int)WishlistGrid.DataKeys[rowIndex].Value;
            int itemIndex = w.Items.IndexOf(wishlistItemId);
            if (itemIndex > -1)
            {
                WishlistItem item = w.Items[itemIndex];
                TextBox desired = (TextBox)saverow.FindControl("Desired");
                DropDownList priority = (DropDownList)saverow.FindControl("Priority");
                TextBox comment = (TextBox)saverow.FindControl("Comment");
				int des = AlwaysConvert.ToInt(desired.Text,item.Desired);
				if(des > System.Int16.MaxValue) des = System.Int16.MaxValue;
                item.Desired = (System.Int16)des;
                item.Priority = AlwaysConvert.ToByte(priority.SelectedValue);
                item.Comment = StringHelper.StripHtml(comment.Text.Trim());
                item.Save();
                rowIndex++;
            }
        }
        WishlistGrid.DataBind();
    }

    protected void KeepShoppingButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(NavigationHelper.GetLastShoppingUrl());
    }

    protected void EmailWishlistButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Members/SendMyWishList.aspx");
    }

    protected void WishlistGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.CommandArgument.ToString()))
        {
            int rowIndex = AlwaysConvert.ToInt(e.CommandArgument);
            int wishlistItemId = (int)WishlistGrid.DataKeys[rowIndex].Value;
            switch (e.CommandName)
            {
                case "Basket":
                    Wishlist wishlist = Token.Instance.User.PrimaryWishlist;
                    int index = wishlist.Items.IndexOf(wishlistItemId);
                    if (index > -1)
                    {
                        wishlist.Items.MoveToBasket(index, Token.Instance.User.Basket);
                        Response.Redirect("~/Basket.aspx");
                    }
                    WishlistGrid.DataBind();
                    break;
            }
        }
    }

    protected void WishlistGrid_DataBound(object sender, System.EventArgs e)
    {
        if ((WishlistGrid.Rows.Count > 0))
        {
            ClearWishlistButton.Visible = true;
            UpdateButton.Visible = true;
            //ONLY SHOW THE SEND LINK IF SMTP SERVER IS UNAVAILABLE
            EmailWishlistButton.Visible = (!String.IsNullOrEmpty(Token.Instance.Store.Settings.SmtpServer));
            if (EmailWishlistButton.Visible && Token.Instance.User.IsAnonymous)
                EmailWishlistButton.OnClientClick = "return confirm('Only registered users can email a wishlist.  You will be asked to login or register if you continue.')";
        }
        else
        {
            ClearWishlistButton.Visible = false;
            UpdateButton.Visible = false;
            EmailWishlistButton.Visible = false;
        }
    }

    protected bool HasKitProducts(object dataItem)
    {
        return !string.IsNullOrEmpty(((WishlistItem)dataItem).KitList);
    }

    protected List<KitProduct> GetKitProducts(object dataItem)
    {
        return ((WishlistItem)dataItem).GetKitProducts(false);
    }

    protected void WishlistItemsDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["wishlistId"] = Token.Instance.User.PrimaryWishlist.WishlistId;
    }

    protected LSDecimal GetPrice(WishlistItem item)
    {
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
        return TaxHelper.GetShopPrice(price, item.Product.TaxCodeId);
    }

    protected int GetWishListId()
    {
        return Token.Instance.User.PrimaryWishlist.WishlistId;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //VALIDATE THE WISHLIST
        List<string> warnMessages;
        bool isValid = Token.Instance.User.PrimaryWishlist.Validate(out warnMessages);
        //DISPLAY ANY WARNING MESSAGES
        if (!isValid)
        {
            WarningMessageList.DataSource = warnMessages;            
            WarningMessageList.DataBind();
        }
    }
}