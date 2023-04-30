using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Shipping;
using MakerShop.Orders;
using MakerShop.Users;
using MakerShop.Utility;

public partial class ConLib_ShipAddressPage : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //PREPARE BASKET FOR CHECKOUT
        Basket basket = Token.Instance.User.Basket;
        //WE DO NOT WANT TO RESET THE GIFT OPTIONS AT THIS STAGE
        basket.Package(false);
        //VALIDATE THE BASKET CONTAINS ITEMS, OTHERWISE REDIRECT
        if (basket.Items.Count == 0) Response.Redirect("~/Basket.aspx");
        //VALIDATE THE BASKET CONTAINS SHIPMENTS, OTHERWISE REDIRECT TO PAYMENT PAGE
        if (basket.Shipments.Count == 0) Response.Redirect("Payment.aspx");
        //PREPARE CAPTION
        ShipToCaption.Text = string.Format(ShipToCaption.Text, Token.Instance.User.PrimaryAddress.FullName);
        //BIND ADDRESSES FOR SELECTION
        InitAddressBook();
        //HIDE MULTIPLE SHIP TO IF ONLY ONE ITEM
        MultipleAddressLink.Visible = (basket.Items.ShippableProductCount > 1);
    }

    private bool ShowMultipleAddresses(Basket basket)
    {
        bool foundItem = false;
        foreach (BasketItem item in basket.Items)
        {
            if (item.Shippable != Shippable.No)
            {
                //IS THIS THE SECOND SHIPPABLE ITEM?
                if (foundItem) return true;
                //IS THERE MORE THAN ONE?
                if (item.Quantity > 1) return true;
                //RECORD THAT A SHIPPABLE ITEM WAS FOUND
                foundItem = true;
            }
        }
        return false;
    }

    protected void InitAddressBook()
    {
        //REARRANGE THE ADDRESSES SO THAT THE PRIMARY ADDRESS DISPLAYS FIRST
        //AND THE REMAINING ADDRESSES ARE IN ALPHABETICAL ORDER BY LAST NAME
        AddressCollection addresses = Token.Instance.User.Addresses;
        addresses.Sort("LastName");
        int defaultIndex = addresses.IndexOf(Token.Instance.User.PrimaryAddressId);
        if (defaultIndex > 0)
        {
            Address tempAddress = addresses[defaultIndex];
            addresses.RemoveAt(defaultIndex);
            addresses.Insert(0, tempAddress);
        }
        //BIND THE ADDRESSES TO THE DATALIST
        ShipToAddressList.DataSource = Token.Instance.User.Addresses;
        ShipToAddressList.DataBind();
    }

    protected void NewAddressButton_Click(object sender, EventArgs e)
    {
        ShowEditShipToPanel(0);
    }

    protected void ShipToAddressList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Pick")
        {
            int addressId = AlwaysConvert.ToInt(e.CommandArgument);
            Basket basket = Token.Instance.User.Basket;
            foreach (BasketShipment shipment in basket.Shipments)
            {
                shipment.AddressId = addressId;
                shipment.Save();
            }
            Response.Redirect("ShipMethod.aspx");
        }
        else if (e.CommandName == "Edit")
        {
            int addressId = AlwaysConvert.ToInt(e.CommandArgument);
            if (addressId != 0) ShowEditShipToPanel(addressId);
        }
    }

    protected void ShowEditShipToPanel(int addressId)
    {
        if (addressId != 0) Response.Redirect("EditShipAddress.aspx?AddressId=" + addressId.ToString());
        Response.Redirect("EditShipAddress.aspx");
    }
}
