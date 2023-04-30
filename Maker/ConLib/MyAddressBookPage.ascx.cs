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
using MakerShop.Orders;
using MakerShop.Users;
using MakerShop.Utility;

public partial class ConLib_MyAddressBookPage : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //PREPARE CAPTION
            String userName = Token.Instance.User.PrimaryAddress.FullName;
            if (String.IsNullOrEmpty(userName)) Caption.Text = "My Address Book";
            else Caption.Text = string.Format(Caption.Text, Token.Instance.User.PrimaryAddress.FullName);

            //BIND ADDRESSES FOR SELECTION
            InitAddressBook();
        }
    }

    protected void InitAddressBook()
    {
        //Clear any address associations with basket items
        Basket basket = Token.Instance.User.Basket;
        basket.Package(true);

        //REARRANGE THE ADDRESSES SO THAT THE PRIMARY ADDRESS DISPLAYS FIRST
        //AND THE REMAINING ADDRESSES ARE IN ALPHABETICAL ORDER BY LAST NAME
        AddressCollection addresses = Token.Instance.User.Addresses;
        addresses.Sort("LastName");
        int defaultIndex = addresses.IndexOf(Token.Instance.User.PrimaryAddressId);
        if (defaultIndex > -1)
        {
            Address tempAddress = addresses[defaultIndex];
            addresses.RemoveAt(defaultIndex);

            // IF THE PRIMARY ADDRESS IS AN EMPTY ADDRESS SHOW A FRIENDLY MESSAGE
            if (!tempAddress.IsValid)
            {
                PrimaryAddress.Text = "Please update your billing address.";
            }
            else
            {
                PrimaryAddress.Text = tempAddress.ToString();
            }
        }
        //BIND THE ADDRESSES TO THE DATALIST
        AddressList.DataSource = addresses;
        AddressList.DataBind();
    }

    protected void NewAddressButton_Click(object sender, EventArgs e)
    {
        ShowEditPanel(0);
    }

    protected void AddressList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Delete")
        {
            int addressId = AlwaysConvert.ToInt(e.CommandArgument);
            if (addressId != 0)
            {
                int index = Token.Instance.User.Addresses.IndexOf(addressId);
                if (index > -1) Token.Instance.User.Addresses.DeleteAt(index);
                InitAddressBook();
            }
        }
        else if (e.CommandName == "Edit")
        {
            int addressId = AlwaysConvert.ToInt(e.CommandArgument);
            if (addressId != 0) ShowEditPanel(addressId);
        }
    }

    protected void ShowEditPanel(int addressId)
    {
        string queryString = (addressId != 0) ? "?AddressId=" + addressId.ToString() : string.Empty;
        Response.Redirect("~/Members/EditMyAddress.aspx" + queryString);
    }

    protected bool IsBillingAddress(object dataItem)
    {
        Address address = (Address)dataItem;
        return (address.AddressId == Token.Instance.User.PrimaryAddressId);
    }
    protected void EditPrimaryAddressButton_Click(object sender, EventArgs e)
    {
        ShowEditPanel(Token.Instance.User.PrimaryAddressId);
    }
}
