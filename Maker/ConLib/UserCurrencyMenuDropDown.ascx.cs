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
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;

public partial class ConLib_UserCurrencyMenuDropDown : System.Web.UI.UserControl
{
    protected void UserCurrency_SelectedIndexChanged(object sender, EventArgs e)
    {
        User user = Token.Instance.User;
        user.UserCurrencyId = AlwaysConvert.ToInt(UserCurrency.SelectedValue);
        user.Save();

		//Redirect After Post. This will ensure currency is update on the current page.
		Response.Redirect(Request.RawUrl);
    }

    protected void UserCurrency_DataBound(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Store store = Token.Instance.Store;
            Currency currency = Token.Instance.User.UserCurrency;
            ListItem item = UserCurrency.Items.FindByValue(currency.CurrencyId.ToString());
            if (item != null)
            {
                item.Selected = true;
            }            
        }
    }
}
