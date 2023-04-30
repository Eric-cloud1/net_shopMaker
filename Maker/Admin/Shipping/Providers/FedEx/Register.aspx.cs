using MakerShop.Shipping.Providers.FedEx;
using System;
using MakerShop.Shipping;
using MakerShop.Utility;
using System.Web;

partial class Admin_Shipping_Providers_FedEx_Default : MakerShop.Web.UI.MakerShopAdminPage {

	protected void RedirectToMain()
	{
     Response.Redirect("../Default.aspx");
	}

	protected void CancelButton_Click(object sender, System.EventArgs e)
	{
        RedirectToMain();
	}

	protected void NextButton_Click(object sender, System.EventArgs e)
	{        
        Response.Redirect("Register1.aspx?Accept=true");
	}

}
